using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TomKerkhove.Dapr.Core.Contracts;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Clients;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Contracts;
using TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing.Interfaces;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing
{
    public class TwinChangedNotificationProcessor : IEventProcessor
    {
        private readonly DeviceRegistryClient _deviceRegistryClient;
        private readonly ILogger<TwinChangedNotificationProcessor> _logger;

        public TwinChangedNotificationProcessor(DeviceRegistryClient deviceRegistryClient, ILogger<TwinChangedNotificationProcessor> logger)
        {
            _deviceRegistryClient = deviceRegistryClient;
            _logger = logger;
        }

        public async Task ProcessAsync(NotificationMetadata notificationMetadata, string rawEvent)
        {
            var twinChangedNotification = JsonConvert.DeserializeObject<TwinChangedNotification>(rawEvent);
            
            await _deviceRegistryClient.NotifyTwinChangedAsync(notificationMetadata.DeviceId, twinChangedNotification);
        }
    }
}