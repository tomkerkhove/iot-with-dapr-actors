using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TomKerkhove.Dapr.Core.Clients;
using TomKerkhove.Dapr.Core.Contracts;
using TomKerkhove.Dapr.Streaming.DeviceTwins.Contracts;
using TomKerkhove.Dapr.Streaming.DeviceTwins.EventProcessing.Interfaces;

namespace TomKerkhove.Dapr.Streaming.DeviceTwins.EventProcessing
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

            // Don't do anything if it's just reported twin updates
            if (twinChangedNotification.Properties.Desired == null && twinChangedNotification.Tags == null)
            {
                _logger.LogInformation("Device twin change ignored as it only contained reported properties");
                return;
            }

            var twinInformation = TwinInformation.Parse(twinChangedNotification);
            
            await _deviceRegistryClient.NotifyTwinChangedAsync(notificationMetadata.DeviceId, twinInformation);

            _logger.LogInformation("Device twin change processed");
        }
    }
}