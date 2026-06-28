using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.DurableFunctions.Entity
{
    public class PaymentResult
    {
        public bool Success { get; set; }

        public string TransactionId { get; set; } = "";
    }
}