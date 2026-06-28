using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.DurableFunctions.Entity;
using AzureFunction.DurableFunctions.OrchestratorFunction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace AzureFunction.DurableFunctions.ClientFunction
{
    public class OrderHttpTrigger
    {
        [Function("StartOrder")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, [DurableClient] DurableTaskClient client)
        {
            var order = await req.ReadFromJsonAsync<Order>();
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrderOrchestrator), order);

            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}