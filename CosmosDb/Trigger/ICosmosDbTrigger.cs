using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunction.CosmosDb.DTO;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.CosmosDb.Trigger
{
    public interface ICosmosDbTrigger
    {
        public Task CreateAIKnowledgeAsync(IReadOnlyList<AuditLogEvent> changes, FunctionContext executionContext);
    }
}