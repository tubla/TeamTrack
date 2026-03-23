using TeamTrack.Api.DTOs.Task;

namespace TeamTrack.Api.DTOs.SignalR
{
    public class TaskEventDto
    {
        public string EventType { get; set; } = string.Empty; // Created, Updated, Deleted, StatusChanged
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public TaskDto? Task { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
        public string? TriggeredBy { get; set; }
    }
}
