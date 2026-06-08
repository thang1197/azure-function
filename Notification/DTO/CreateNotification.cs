using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Notification.DTO
{
    public class CreateNotification
    {
        public required string Title { get; set; }
        public required string Description { get; set; }

        public required string Sender { get; set; }
        public required string Receiver { get; set; }

        public required string Type { get; set; }
    }
}