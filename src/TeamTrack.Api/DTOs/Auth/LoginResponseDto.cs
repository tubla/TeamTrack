using TeamTrack.Api.DTOs.Organization;

namespace TeamTrack.Api.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = new();
    public List<OrganizationDto> Organizations { get; set; } = [];
}
