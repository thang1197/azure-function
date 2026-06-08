using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Tasks.DTO.Database
{
    public class CreateTask(string title, string description, DateTime dueDate, string userUpdated)
    {
        public string Title { get; set; } = title;
        public string Description { get; set; } = description;
        public DateTime DueDate { get; set; } = dueDate;
        public string UserUpdated { get; set; } = userUpdated;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}