using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using GuardNet;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Core.Actors.Device.Contracts;
using TomKerkhove.Dapr.Core.Actors.Device.Interface;
using TomKerkhove.Dapr.Core.Contracts;

namespace TomKerkhove.Dapr.APIs.Management.Repositories
{
    public class DeviceRepository
    {
        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(ILogger<DeviceRepository> logger)
        {
            Guard.NotNull(logger,nameof(logger));

            _logger = logger;
        }

        public async Task<DeviceInfo> GetDataAsync(string deviceId)
        {
            return await Interact(deviceId, deviceActor => deviceActor.GetInfoAsync());
        }

        public async Task SetDataAsync(string deviceId, DeviceInfo newData)
        {
            await Interact(deviceId, deviceActor => deviceActor.SetInfoAsync(newData));
        }

        public async Task ReceiveMessageAsync(string deviceId, MessageTypes messageType, string payload)
        {
            await Interact(deviceId, deviceActor => deviceActor.ReceiveMessageAsync(messageType, payload));
        }

        public async Task ReportPropertiesAsync(string deviceId, Dictionary<string, string> twin)
        {
            await Interact(deviceId, deviceActor => deviceActor.SetReportedPropertyAsync(twin));
        }

        public async Task NotifyTwinInformationChangedAsync(string deviceId, TwinInformation twinChangedNotification)
        {
            await Interact(deviceId, deviceActor => deviceActor.NotifyTwinInformationChangedAsync(twinChangedNotification));
        }

        public async Task<Dictionary<string, string>> GetTagsAsync(string deviceId)
        {
            return await Interact(deviceId, deviceActor => deviceActor.GetTagsAsync());
        }

        private async Task Interact(string deviceId, Func<IDeviceActor, Task> interactionFunc)
        {
            var proxy = CreateActorProxy(deviceId);
            
            try
            {
                await interactionFunc(proxy);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to interact with Actor proxy");
                throw;
            }
        }

        private async Task<TResult> Interact<TResult>(string deviceId, Func<IDeviceActor, Task<TResult>> interactionFunc)
        {
            var proxy = CreateActorProxy(deviceId);

            try
            {
                return await interactionFunc(proxy);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to interact with Actor proxy");
                throw;
            }
        }

        private IDeviceActor CreateActorProxy(string deviceId)
        {
            try
            {
                var actorId = new ActorId(deviceId);

                // Create the local proxy by using the same interface that the service implements
                // By using this proxy, you can call strongly typed methods on the interface using Remoting.
                var proxy = ActorProxy.Create<IDeviceActor>(actorId, "DeviceActor");
                return proxy;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failed to create Actor proxy");
                throw;
            }
        }
    }
}