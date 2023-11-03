using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;


namespace TomKerkhove.Dapr.Streaming.Runtimes.AzureFunctions.Functions
{
    public abstract class AzureEventHubsFunction
    {
        /// <summary>
        /// Processes an individual event
        /// </summary>
        /// <param name="eventData">Full event metadata and payload provided by Azure Event Hubs</param>
        /// <param name="rawEventPayload">Raw payload of the event</param>
        protected abstract Task ProcessIndividualEventAsync(EventData eventData, string rawEventPayload);

        /// <summary>
        /// Processes all events in the batch
        /// </summary>
        /// <param name="events">Received event batch</param>
        protected virtual async Task ProcessEventsAsync(EventData[] events)
        {
            var exceptions = new List<Exception>();

            var allEventTasks = new List<Task>();
            foreach (EventData eventData in events)
            {
                var task = ProcessIndividualEventAsync(eventData, exceptions);
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

        /// <summary>
        /// Processes an individual event with required safeguarding
        /// </summary>
        /// <param name="eventData">Full event metadata and payload provided by Azure Event Hubs</param>
        /// <param name="exceptions">List of exceptions that are occuring for the batch</param>
        protected virtual async Task ProcessIndividualEventAsync(EventData eventData, List<Exception> exceptions)
        {
            try
            {
                if (eventData.Body.IsEmpty || eventData.Body.ToArray().Length == 0)
                {
                    throw new Exception("Message does not contain a payload");
                }

                string rawEventPayload = Encoding.UTF8.GetString(eventData.Body.Span);

                await ProcessIndividualEventAsync(eventData, rawEventPayload);
            }
            catch (Exception exception)
            {
                exceptions.Add(exception);
            }
        }
    }
}
