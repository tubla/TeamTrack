using TeamTrack.Api.DTOs.Organization;

namespace TeamTrack.Api.DTOs.Profile;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public OrganizationDto? CurrentOrganization { get; set; }
    public List<string> Permissions { get; set; } = [];
}
