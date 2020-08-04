using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.DeviceTwin.Monitor.Contracts;
using TomKerkhove.Dapr.DeviceTwin.Monitor.EventProcessing;

namespace TomKerkhove.Dapr.DeviceTwin.Monitor.Functions
{
    public class TwinChangeMonitorFunction
    {
        private readonly EventProcessor _eventProcessor;

        public TwinChangeMonitorFunction(EventProcessor eventProcessor)
        {
            Guard.NotNull(eventProcessor,nameof(eventProcessor));

            _eventProcessor = eventProcessor;
        }

        [FunctionName("twin-change-monitor")]
        public async Task Run([EventHubTrigger("twin-changes", Connection = "TwinEventHubs")] EventData[] events, ILogger logger)
        {
            var exceptions = new List<Exception>();

            var allEventTasks = new List<Task>();
            foreach (EventData eventData in events)
            {
                var task = ProcessEventAsync(eventData, logger, exceptions);
                allEventTasks.Add(task);
            }

            await Task.WhenAll(allEventTasks);

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
            { 
                throw new AggregateException(exceptions);
            }

            if (exceptions.Count == 1)
            { 
                throw exceptions.Single();
            }
        }

        private async Task ProcessEventAsync(EventData eventData, ILogger logger, List<Exception> exceptions)
        {
            try
            {
                if (eventData.Body == null || eventData.Body.Array == null)
                {
                    throw new Exception("Message does not contain a payload");
                }

                string rawEvent = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                var notificationMetadata = NotificationMetadata.Parse(eventData);

                await _eventProcessor.ProcessAsync(notificationMetadata.MessageSchema, notificationMetadata, rawEvent);
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }
    }
}
