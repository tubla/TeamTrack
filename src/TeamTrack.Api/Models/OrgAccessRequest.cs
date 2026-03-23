namespace TeamTrack.Api.Models
{
    public class OrgAccessRequest : BaseEntity
    {
        public string Email { get; set; } = default!;
        public string? Message { get; set; }
        public OrgAccessRequestStatus Status { get; set; } = OrgAccessRequestStatus.Pending;
        public DateTimeOffset? ProcessedAt { get; set; }
        public Guid? ProcessedBy { get; set; }
    }
}
