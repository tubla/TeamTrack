namespace TeamTrack.Api.Models
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;

        public bool IsRead { get; set; } = false;

        public NotificationType Type { get; set; }

        public Guid? ReferenceId { get; set; } // Task / Project / etc.
    }
}
