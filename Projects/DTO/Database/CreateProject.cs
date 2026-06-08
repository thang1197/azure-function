using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure_Function.Projects.DTO.Database
{
    public class CreateProject
    {
        public string Name { get; set; } = string.Empty;

        public required string Description { get; set; }

        public required string Owner { get; set; }

        public required string Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}