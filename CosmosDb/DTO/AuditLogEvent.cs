using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzureFunction.CosmosDb.DTO
{
    public class AuditLogEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("eventId")]
        public string EventId { get; set; } = string.Empty;

        [JsonProperty("taskTitle")]
        public required string TaskTitle { get; set; }

        [JsonProperty("projectId")]
        public int ProjectId { get; set; }

        [JsonProperty("taskId")]
        public int TaskId { get; set; }

        [JsonProperty("action")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public AuditAction Action { get; set; }

        [JsonProperty("actor")]
        public string Actor { get; set; } = string.Empty;

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("changes")]
        public List<FieldChange> Changes { get; set; } = [];
    }

    public class FieldChange
    {
        [JsonProperty("field")]
        public string Field { get; set; } = string.Empty;

        [JsonProperty("oldValue")]
        public string? OldValue { get; set; }

        [JsonProperty("newValue")]
        public string? NewValue { get; set; }
    }

    public enum AuditAction
    {
        TaskCreated,
        TaskUpdated,
        TaskStatusChanged,
        TaskAssigned,
        TaskUnassigned,
        TaskDeleted,
        TaskRestored,
        TaskCommentAdded,
        TaskAttachmentAdded,
        TaskDueDateChanged
    }
}