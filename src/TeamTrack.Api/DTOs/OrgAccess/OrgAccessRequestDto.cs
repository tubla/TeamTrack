namespace TeamTrack.Api.DTOs.OrgAccess
{
    public class OrgAccessRequestDto
    {
        public string Email { get; set; } = default!;
        public string? Message { get; set; }
    }
}
