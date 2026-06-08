using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Tasks.DTO.Http
{
    public class CreateTaskRequest
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; } 
    }
}