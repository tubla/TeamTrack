namespace TeamTrack.Api.Models
{
    public class ActivityLog : BaseEntity
    {
        public Guid OrganizationId { get; set; }

        public Guid UserId { get; set; }

        public string Action { get; set; } = default!;
        public string EntityType { get; set; } = default!;
        public Guid EntityId { get; set; }

        public string? Metadata { get; set; } // JSON (flexible)
    }
}
