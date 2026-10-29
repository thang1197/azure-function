using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunction.MessageQueue.DTO;
using Microsoft.Extensions.Logging;

namespace AzureFunction.MessageQueue.Service
{
    public class ServiceBusProducerService(ILogger<ServiceBusProducerService> logger, ServiceBusSender serviceBusSender) : IProducerService
    {
        private readonly ILogger<ServiceBusProducerService> _logger = logger;

        private readonly ServiceBusSender _serviceBusSender = serviceBusSender;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task SendMessageAsync<T>(Message<T> message, CancellationToken cancellationToken = default)
        {
            try
            {
                var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(message.Body, _jsonSerializerOptions))
                {
                    MessageId = message.Id,
                    ContentType = "application/json",
                    Subject = message.Subject
                };
                
                await _serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
        }
    }
}