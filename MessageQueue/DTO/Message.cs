using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.MessageQueue.DTO
{
    public class Message<T>
    {
        public required string Id { get; set; }

        public required string Subject { get; set; }

        public required T Body { get; set; } 
    }
}