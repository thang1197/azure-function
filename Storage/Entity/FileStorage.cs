using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Storage.Entity
{
    [Table("FileStorage")]
    public class FileStorage
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public required string Name { get; set; }

        [Column("url")]
        public required string Url { get; set; }

        [Column("size")]
        public long Size { get; set; }

        [Column("content_type")]
        public required string ContentType { get; set; }

        [Column("version")]
        public string Version { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("user_created")]
        public string UserCreated { get; set; } = string.Empty;

        [Column("user_updated")]
        public string UserUpdated { get; set; } = string.Empty;
    }
}