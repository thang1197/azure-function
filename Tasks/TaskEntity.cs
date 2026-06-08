using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Tasks
{
    public class TaskEntity(int id, string title, string description, DateTime dueDate)
    {
        public int Id { get; set; } = id;
        public string Title { get; set; } = title;
        public string Description { get; set; } = description;
        public DateTime DueDate { get; set; } = dueDate;
    }
}