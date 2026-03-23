using TeamTrack.Api.DTOs.Project;

namespace TeamTrack.Api.DTOs.SignalR
{
    public class ProjectEventDto
    {
        public string EventType { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
        public ProjectDto? Project { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
