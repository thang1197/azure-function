using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Azure_Function.Projects
{
    [Table("Projects")]
    public class Project
    {
        [Key] // Defines 'id' as the Primary Key
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        [Required]
        [MaxLength(255)] // Matches character varying(255)
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column("owner")]
        [MaxLength(255)]
        public string? Owner { get; set; }

        [Column("status")]
        public required string Status { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    // Enum matching your PostgreSQL "ProjectStatus" type
    // (Modify these enum values to match your actual database enum labels)
    public enum ProjectStatus
    {
        TO_DO,
        DONE,
        IN_PROGRESS
    }
}