using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace AzureFunction.DurableFunctions.OrchestratorFunction
{
    public class TaskWorker
    {
        [Function("SendEmailFunction")]
        public async Task SendEmailFunction([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            await context.CallActivityAsync("SendEmail", "Hello");
        }
    }
}