using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using GuardNet;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Core.Clients;
using TomKerkhove.Dapr.Core.Contracts;
using TomKerkhove.Dapr.Streaming.Runtimes.AzureFunctions.Functions;

namespace TomKerkhove.Dapr.Streaming.DeviceTelemetry.Functions
{
    public class MessageProcessingFunction : AzureEventHubsFunction
    {
        private const string DeviceId = "deviceId";
        private const string MessageType = "messageType";
        private readonly DeviceRegistryClient _deviceRegistryClient;

        public MessageProcessingFunction(DeviceRegistryClient deviceRegistryClient)
        {
            Guard.NotNull(deviceRegistryClient, nameof(deviceRegistryClient));

            _deviceRegistryClient = deviceRegistryClient;
        }

        [FunctionName("device-message-processor")]
        public async Task Run([EventHubTrigger("device-messages", Connection = "EventHubs.ConnectionStrings.DeviceMessages")] EventData[] events, ILogger logger)
        {
            await ProcessEventsAsync(events);
        }

        protected override async Task ProcessIndividualEventAsync(EventData eventData, string rawEventPayload)
        {
            var deviceId = eventData.Properties[DeviceId]?.ToString();
            var rawMessageType = eventData.Properties[MessageType]?.ToString();
            
            if(Enum.TryParse(rawMessageType, ignoreCase: true, result: out MessageTypes messageType))
            {
                await _deviceRegistryClient.SendMessageAsync(deviceId, messageType, rawEventPayload);
            }
            else
            {
                throw new Exception($"Unable to process message with type {rawMessageType}");
            }
        }


    }
}
