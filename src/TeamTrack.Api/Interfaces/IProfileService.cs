using TeamTrack.Api.DTOs.Profile;

namespace TeamTrack.Api.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync();
    Task<ProfileDto> UpdateProfileAsync(UpdateProfileDto dto);
    Task ChangePasswordAsync(ChangePasswordDto dto);
}
