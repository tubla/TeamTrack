namespace TeamTrack.Api.DTOs.Profile
{
    public class UserOrganizationDto
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTimeOffset JoinedAt { get; set; }
    }
}
