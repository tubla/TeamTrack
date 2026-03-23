namespace TeamTrack.Api.DTOs.SignalR
{

    public class NotificationDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ReferenceId { get; set; } = null;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsRead { get; set; }
    }
}
