namespace TeamTrack.Api.DTOs.Auth
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Guid? CurrentOrganizationId { get; set; }
        public string? CurrentOrganizationName { get; set; }
        public List<string> Permissions { get; set; } = [];
        public List<string> Roles { get; set; } = [];
    }
}
