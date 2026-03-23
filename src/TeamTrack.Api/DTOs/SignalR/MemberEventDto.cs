using TeamTrack.Api.DTOs.Member;

namespace TeamTrack.Api.DTOs.SignalR
{
    public class MemberEventDto
    {
        public string EventType { get; set; } = string.Empty; // Joined, Removed
        public Guid UserId { get; set; }
        public MemberDto? Member { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
