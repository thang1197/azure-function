using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunction.CosmosDb.DTO;
using AzureFunction.MessageQueue.DTO;
using AzureFunction.MessageQueue.Service;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunction.CosmosDb.Trigger
{
    public class CosmosDbTrigger(
        ILogger<CosmosDbTrigger> logger,
        ServiceBusClient serviceBusClient
        ) : 
        ServiceBusProducerService(logger, serviceBusClient.CreateSender("tasks", new ServiceBusSenderOptions())), ICosmosDbTrigger
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
        {
            PropertyNameCaseInsensitive = true
        };
        private readonly ILogger<CosmosDbTrigger> _logger = logger;
        [Function("CreateAIKnowledgeAsync")]
        // [ServiceBusOutput("tasks", Connection = "AzureWebJobsServiceBusStandard")]
        public async Task CreateAIKnowledgeAsync([CosmosDBTrigger(
            databaseName: "TaskManagement",
            containerName: "AuditLogs",
            Connection = "CosmosDBConnection",
            LeaseConnection = "CosmosDBServerlessConnection",
            LeaseDatabaseName = "TaskManagement",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)]
            IReadOnlyList<AuditLogEvent> changes,
            FunctionContext executionContext)
        {
            try
            {
                foreach (var item in changes)
                {
                    var id = item.Id;
                    _logger.LogInformation("Changed doc id: {id}", id);

                    // Dump the whole document as JSON
                    _logger.LogInformation("Raw: {json}", JsonSerializer.Serialize(item, _jsonSerializerOptions));
                    
                    // var message = new ServiceBusMessage(JsonSerializer.Serialize(item, _jsonSerializerOptions))
                    // {
                    //     ContentType = "application/json",
                    //     MessageId = item.EventId,
                    //     PartitionKey = item.TaskId.ToString(),
                    //     Subject = "AIKnowledge",
                    // };

                    // messages.Add(message);
                    var message = new Message<AuditLogEvent>()    
                    {
                        Id = item.EventId,
                        Subject = "AIKnowledge",
                        Body = item
                    };
                    await SendMessageAsync<AuditLogEvent>(message);
                }
                // return messages;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error CreateAIKnowledgeAsync");
                throw;
            }
        }
    }
}