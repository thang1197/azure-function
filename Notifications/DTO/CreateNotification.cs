using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Notifications.DTO
{
    public class CreateNotification
    {
        [Required(ErrorMessage = "Title is required.")]
        public required string Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Sender is required.")]
        public required string Sender { get; set; }
        
        [Required(ErrorMessage = "Receiver is required.")]
        public required string Receiver { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public required string Type { get; set; }
    }
}