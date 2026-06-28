using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.DurableFunctions.ActivityFunction;
using AzureFunction.DurableFunctions.Entity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureFunction.DurableFunctions.OrchestratorFunction
{
    public class OrderOrchestrator(ILogger<OrderOrchestrator> logger)
    {
        private readonly ILogger<OrderOrchestrator> _logger = logger;
        [Function(nameof(OrderOrchestrator))]
        public async Task Run([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var order = context.GetInput<Order>();
            order = await context.CallActivityAsync<Order>(nameof(OrderActivities.ValidateOrder), order);
            order = await context.CallActivityAsync<Order>(nameof(OrderActivities.ReserveInventory), order);
            order = await context.CallActivityAsync<Order>(nameof(OrderActivities.ProcessPayment), order);

            _logger.LogInformation("Order processed successfully: {orderId}", order.OrderId);
        }
    }
}