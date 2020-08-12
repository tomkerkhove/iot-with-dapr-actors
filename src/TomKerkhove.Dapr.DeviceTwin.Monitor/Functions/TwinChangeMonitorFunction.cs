using System.Threading.Tasks;
using GuardNet;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Contracts;
using TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing;
using TomKerkhove.Dapr.Runtimes.AzureFunctions.Functions;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.Functions
{
    public class TwinChangeMonitorFunction : AzureEventHubsFunction
    {
        private readonly EventProcessor _eventProcessor;

        public TwinChangeMonitorFunction(EventProcessor eventProcessor)
        {
            Guard.NotNull(eventProcessor,nameof(eventProcessor));

            _eventProcessor = eventProcessor;
        }

        [FunctionName("twin-change-monitor")]
        public async Task Run([EventHubTrigger("twin-changes", Connection = "EventHubs.ConnectionStrings.TwinChanges")] EventData[] events, ILogger logger)
        {
            await ProcessEventsAsync(events);
        }

        protected override async Task ProcessIndividualEventAsync(EventData eventData, string rawEventPayload)
        {
            var notificationMetadata = NotificationMetadata.Parse(eventData);

            await _eventProcessor.ProcessAsync(notificationMetadata.MessageSchema, notificationMetadata, rawEventPayload);
        }
    }
}
