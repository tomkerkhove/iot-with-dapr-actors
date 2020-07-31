using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;
using TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing.Interfaces;

namespace TomKerkhove.Dapr.Actors.Runtime.Device.MessageProcessing
{
    public class MessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;
        public MessageProcessor(ILogger<MessageProcessor> logger)
        {
            _logger = logger;
        }

        public async Task ProcessAsync(MessageTypes messageType, string rawMessage)
        {
            IMessageProcessor processor;

            switch (messageType)
            {
                case MessageTypes.Telemetry:
                    processor = new TelemetryMessageProcessor(_logger);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unable to process message of type {messageType}");
            }

            await processor.ProcessAsync(rawMessage);
        }
    }
}
