using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Azure_Function.Projects.DTO.Http
{
    public class CreateProjectsRequest
    {
        public string Name { get; set; } = string.Empty;

        public required string Description { get; set; }

        public required string Owner { get; set; }

        // This attribute tells .NET to successfully read "IN_PROGRESS" or "DONE" as strings
        // [JsonConverter(typeof(JsonStringEnumConverter))]
        public required string Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}