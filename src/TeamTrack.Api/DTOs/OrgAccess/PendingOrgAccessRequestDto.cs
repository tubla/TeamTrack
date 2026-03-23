namespace TeamTrack.Api.DTOs.OrgAccess
{
    public class PendingOrgAccessRequestDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string? Message { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public Models.OrgAccessRequestStatus Status { get; set; }
    }
}
