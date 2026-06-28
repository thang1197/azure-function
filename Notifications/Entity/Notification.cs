using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Notifications.Entity
{
    [Table("Notifications")]
    public class Notification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("title")]
        public required string Title { get; set; }

        [Required]
        [Column("description")]
        public required string Description { get; set; }

        [Required]
        [Column("sender")]
        public required string Sender { get; set; }

        [Required]
        [Column("receiver")]
        public required string Receiver { get; set; }
        
        [Required]
        [Column("status")]
        public required string Status { get; set; }

        [Required]
        [Column("type")]
        public required string Type { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}