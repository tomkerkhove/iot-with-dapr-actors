using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcus.Security.Core;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using Microsoft.Azure.Devices.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Actors.Device.Interface;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;
using TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing;
using TomKerkhove.Dapr.Actors.Runtime.Extensions;

namespace TomKerkhove.Dapr.Actors.Runtime.Actors
{
    internal class DeviceActor : ExtendedActor<DeviceActor>, IDeviceActor, IRemindable
    {
        private const string DeviceInfoKey = "device_info";
        private const string TwinStateKey = "twin_state";

        public string DeviceId => Id.GetId();

        private DeviceClient _deviceClient;

        /// <summary>
        ///     Initializes a new instance of DeviceActor
        /// </summary>
        /// <param name="actorService">The actor service that will host this actor instance.</param>
        /// <param name="actorId">The id for this actor instance.</param>
        public DeviceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task ProvisionAsync(DeviceInfo info)
        {
            await SetInfoAsync(info);
            // TODO: Emit event

            TrackDeviceProvisionedEvent(info);
        }

        private void TrackDeviceProvisionedEvent(DeviceInfo info)
        {
            var contextualInformation = ComposeRequiredContextualInformation();
            contextualInformation.Add("Tenant", info.Tenant);
            contextualInformation.Add("IP", info.IP);
            contextualInformation.Add("IMEI", info.IMEI);

            Logger.LogEvent("Device Provisioned", contextualInformation);
            Logger.LogMetric("Device Provisioned", 1, contextualInformation);
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

        public async Task ReceiveMessageAsync(MessageTypes type, string rawMessage)
        {
            var contextualInformation = ComposeRequiredContextualInformation();
            Logger.LogMetric("Device Message Received", 1, contextualInformation);

            await SetReminderToDetectDeviceGoingOfflineAsync();

            var messageProcessor = Services.GetService<MessageProcessor>();
            await messageProcessor.ProcessAsync(type, rawMessage);
        }

        private async Task SetReminderToDetectDeviceGoingOfflineAsync()
        {
            await UnregisterReminderAsync(ReminderTypes.LastMessageRecieved.ToString());
            await RegisterReminderAsync(ReminderTypes.LastMessageRecieved.ToString(), state: null, dueTime: TimeSpan.FromMinutes(1), TimeSpan.FromMilliseconds(-1));
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

                Logger.LogInformation("Device {DeviceId} activated", DeviceId);
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Unable to active device {DeviceId}", DeviceId);
            }
        }

        private async Task<DeviceClient> CreateIoTHubDeviceClient()
        {
            var secretProvider = Services.GetRequiredService<ISecretProvider>();
            var connectionString = await secretProvider.GetRawSecretAsync($"IOTHUB_CONNECTIONSTRING_DEVICE_{DeviceId}");

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
                _deviceClient.Dispose();
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
            if (isKnownReminder)
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
            Logger.LogMetric("Device Inactive", 1, contextualInformation);
            // TODO: Emit event grid event
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