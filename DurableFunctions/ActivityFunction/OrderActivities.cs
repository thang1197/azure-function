using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.DurableFunctions.Entity;
using AzureFunction.Utility;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunction.DurableFunctions.ActivityFunction
{
    public class OrderActivities
    {
        [Function(nameof(ValidateOrder))]
        public async Task<Order> ValidateOrder([ActivityTrigger] Order order, CancellationToken cancellationToken)
        {
            var randomTimeout = RandomHelper.GetRandomNumber(1, 3) * 1000;
            await Task.Delay(randomTimeout, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Validating Order {order.OrderId}");
            return order;
        }

        [Function(nameof(ReserveInventory))]
        public async Task<Order> ReserveInventory([ActivityTrigger] Order order, CancellationToken cancellationToken)
        {
            var randomTimeout = RandomHelper.GetRandomNumber(1, 3) * 1000;
            await Task.Delay(randomTimeout, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine(
                $"Inventory reserved for Order {order.OrderId}");
            return order;
        }

        [Function(nameof(ProcessPayment))]
        public async Task<PaymentResult> ProcessPayment([ActivityTrigger] Order order, CancellationToken cancellationToken)
        {
            var randomTimeout = RandomHelper.GetRandomNumber(1, 3) * 1000;
            await Task.Delay(randomTimeout, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine(
                $"Charging ${order.Total}");
            return new PaymentResult
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString()
            };
        }
    }
}
