using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcus.Security.Core;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using GuardNet;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing;
using TomKerkhove.Dapr.Actors.Runtime.Enums;
using TomKerkhove.Dapr.Actors.Runtime.Extensions;
using TomKerkhove.Dapr.Core.Actors.Device.Interface;
using TomKerkhove.Dapr.Core.Contracts;
using Message = TomKerkhove.Dapr.Core.Contracts.Message;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

namespace TomKerkhove.Dapr.Actors.Runtime.Actors
{
    internal class DeviceActor : Actor, IDeviceActor, IRemindable
    {
        private const string DeviceInfoKey = "device_info";
        private const string DesiredPropertiesStateKey = "twin_state_desired";
        private const string ReportedPropertiesStateKey = "twin_state_reported";
        private const string TagStateKey = "twin_state_tags";

        public string DeviceId => Id.GetId();

        private readonly MessageProcessor _messageProcessor;
        private readonly ISecretProvider _secretProvider;
        private DeviceClient _deviceClient;

        /// <summary>
        ///     Initializes a new instance of DeviceActor
        /// </summary>
        public DeviceActor(ActorHost actorHost, MessageProcessor messageProcessor, ISecretProvider secretProvider)
            : base(actorHost)
        {
            Guard.NotNull(messageProcessor, nameof(messageProcessor));
            Guard.NotNull(secretProvider, nameof(secretProvider));

            _secretProvider = secretProvider;
            _messageProcessor = messageProcessor;
        }

        public async Task ProvisionAsync(DeviceInfo info, TwinInformation initialTwinInfo)
        {
            await SetInfoAsync(info);

            Logger.LogEvent("Device Provisioned");

            TrackDeviceProvisionedEvent(info);
        }

        private void TrackDeviceProvisionedEvent(DeviceInfo info)
        {
            var contextualInformation = ComposeRequiredContextualInformation();
            contextualInformation.Add("Tenant", info.Tenant);
            contextualInformation.Add("IP", info.IP);
            contextualInformation.Add("IMEI", info.IMEI);

            Logger.LogEvent("Device Provisioned", contextualInformation);
            LogMetric("Device Provisioned", 1, contextualInformation);
        }

        public async Task IpAddressHasChangedAsync(string newIpAddress)
        {
            var deviceInfo = await GetInfoAsync();
            var oldIpAddress = deviceInfo.IP;
            deviceInfo.IP = newIpAddress;
            await StateManager.SetStateAsync(DeviceInfoKey, deviceInfo);

            var contextualInformation = new Dictionary<string, object>
            {
                { "New IP Address", newIpAddress },
                { "Old IP Address", oldIpAddress },
            };
            Logger.LogEvent("Device IP Address Changed", contextualInformation);
        }

        public async Task SetInfoAsync(DeviceInfo data)
        {
            await StateManager.SetStateAsync(DeviceInfoKey, data);
        }

        public async Task<DeviceInfo> GetInfoAsync()
        {
            var potentialData = await StateManager.TryGetStateAsync<DeviceInfo>(DeviceInfoKey);
            return potentialData.HasValue ? potentialData.Value : null;
        }

        public async Task ReceiveMessageAsync(MessageTypes type, Message message)
        {
            var context = new Dictionary<string, object>
            {
                {"MessageType", type}
            };
            LogMetric("Device Message Received", 1, context);

            await SetReminderToDetectDeviceGoingOfflineAsync();
            
            await _messageProcessor.ProcessAsync(type, message.Content);
        }

        public async Task NotifyTwinInformationChangedAsync(TwinInformation notification)
        {
            // TODO: Patch, not overwrite
            await StateManager.AddOrUpdateStateAsync(ReportedPropertiesStateKey, notification.Properties.Reported, (stateName, currentValue) => notification.Properties.Reported);
            await StateManager.AddOrUpdateStateAsync(DesiredPropertiesStateKey, notification.Properties.Desired, (stateName, currentValue) => notification.Properties.Desired);
            await StateManager.AddOrUpdateStateAsync(TagStateKey, notification.Tags, (stateName, currentValue) => notification.Tags);
        }

        public async Task<Dictionary<string, string>> GetTagsAsync()
        {
            var potentialRawTags = await StateManager.TryGetStateAsync<Dictionary<string, string>>(TagStateKey);
            return potentialRawTags.HasValue ? potentialRawTags.Value : new Dictionary<string, string>();
        }

        private async Task SetReminderToDetectDeviceGoingOfflineAsync()
        {
            await UnregisterReminderAsync(ReminderTypes.LastMessageRecieved.ToString());
            await RegisterReminderAsync(ReminderTypes.LastMessageRecieved.ToString(), state: null, dueTime: TimeSpan.FromMinutes(5), TimeSpan.FromMilliseconds(-1));
        }

        public async Task SetReportedPropertyAsync(Dictionary<string,string> reportedProperties)
        {
            try
            {
                var twinInformation = reportedProperties.ConvertToTwinCollection();
                await _deviceClient.UpdateReportedPropertiesAsync(twinInformation);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Unable to set reported properties ({ReportedProperties})", reportedProperties);
            }
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            Logger.LogInformation("Activating device {DeviceId}", DeviceId);

            try
            {
                _deviceClient = await CreateIoTHubDeviceClient();

                // Ensure we are aware of connection changes
                _deviceClient.SetConnectionStatusChangesHandler(DeviceStatusChanged);

                Logger.LogInformation("Device {DeviceId} activated", DeviceId);

                LogMetric("Actor Activated", 1);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Unable to active device {DeviceId}", DeviceId);
            }
        }

        private async Task<DeviceClient> CreateIoTHubDeviceClient()
        {
            var connectionString = await _secretProvider.GetRawSecretAsync($"IOTHUB_CONNECTIONSTRING_DEVICE_{DeviceId}");

            return DeviceClient.CreateFromConnectionString(connectionString, TransportType.Amqp);
        }

        /// <summary>
        ///     This method is called whenever an actor is deactivated after a period of inactivity.
        /// </summary>
        protected override Task OnDeactivateAsync()
        {
            Logger.LogInformation("Deactivating device {DeviceId}", DeviceId);

            try
            {
                LogMetric("Actor Deactivated", 1);

                _deviceClient?.Dispose();
            }
            finally
            {
                Logger.LogInformation("Device {DeviceId} deactivated.", DeviceId);
            }

            return Task.CompletedTask;
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var isKnownReminder = Enum.TryParse(reminderName, out ReminderTypes reminderType);
            if (isKnownReminder == false)
            {
                Logger.LogWarning("Reminder was received for {ReminderName} which is not known and unhandled", reminderName);
                return Task.CompletedTask;
            }

            switch (reminderType)
            {
                case ReminderTypes.LastMessageRecieved:
                    DeviceIsInactive();
                    break;
            }
            
            return Task.CompletedTask;
        }

        private void DeviceIsInactive()
        {
            var contextualInformation = ComposeRequiredContextualInformation();

            Logger.LogEvent("Device Inactive", contextualInformation);
            LogMetric("Device Inactive", 1, contextualInformation);
            // TODO: Emit event grid event
        }

        private void DeviceStatusChanged(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            Logger.LogWarning("Device connection changed to {DeviceConnectionStatus}. Reason {Reason}", status, reason);

            var telemetryContext = ComposeRequiredContextualInformation();
            telemetryContext.TryAdd("Status", status);
            telemetryContext.TryAdd("Reason", reason);
            Logger.LogEvent("Device Connectivity Changed", telemetryContext);
        }

        private void LogMetric(string metricName, double metricValue , Dictionary<string, object> contextualInformation = null)
        {
            var requiredContextualInformation = ComposeRequiredContextualInformation();
            
            contextualInformation ??= new Dictionary<string, object>();

            foreach (var requiredContextInfo in requiredContextualInformation)
            {
                contextualInformation.TryAdd(requiredContextInfo.Key, requiredContextInfo.Value);
            }

            Logger.LogMetric(metricName, metricValue);
        }

        private Dictionary<string, object> ComposeRequiredContextualInformation()
        {
            var contextualInformation = new Dictionary<string, object>
            {
                {"DeviceId", DeviceId}
            };

            return contextualInformation;
        }
    }
}