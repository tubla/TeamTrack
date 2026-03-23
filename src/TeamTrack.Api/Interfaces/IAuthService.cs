using TeamTrack.Api.DTOs.Auth;
using TeamTrack.Api.DTOs.Organization;
using TeamTrack.Api.DTOs.Token;

namespace TeamTrack.Api.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto);
    Task<SwitchOrganizationResponseDto> SwitchOrganizationAsync(Guid userId, SwitchOrganizationDto dto);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(Guid userId, Guid? organizationId);
}