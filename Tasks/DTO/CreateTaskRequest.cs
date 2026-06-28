using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunction.Tasks.DTO
{
    public class CreateTaskRequest
    {
        public string TaskName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Username { get; set; } = string.Empty;

        public int ProjectId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}