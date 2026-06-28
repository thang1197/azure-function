using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Products.DTO
{
    public class SubscriptionRequest
    {
        public required string ProductId { get; set; }
    }
}