using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.DurableFunctions.Entity
{
    public class Order
    {
        public int OrderId { get; set; }

        public string CustomerName { get; set; } = "";

        public string Email { get; set; } = "";

        public decimal Total { get; set; }
    }
}