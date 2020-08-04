using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using GuardNet;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Core.Actors.Device.Contracts;
using TomKerkhove.Dapr.Core.Actors.Device.Interface;

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
            var proxy = CreateActorProxy(deviceId);
            return await proxy.GetInfoAsync();
        }

        public async Task SetDataAsync(string deviceId, DeviceInfo newData)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.SetInfoAsync(newData);
        }

        public async Task ReceiveMessageAsync(string deviceId, MessageTypes messageType, string payload)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.ReceiveMessageAsync(messageType, payload);
        }

        public async Task ReportPropertiesAsync(string deviceId, Dictionary<string, string> twin)
        {
            var proxy = CreateActorProxy(deviceId);
            await proxy.SetReportedPropertyAsync(twin);
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