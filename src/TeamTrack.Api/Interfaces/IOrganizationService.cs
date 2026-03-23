using TeamTrack.Api.DTOs.Organization;

namespace TeamTrack.Api.Interfaces
{
    public interface IOrganizationService
    {
        Task<object> CreateAsync(CreateOrganizationDto dto);
        Task<List<OrganizationDto>> GetUserOrganizationsAsync();
        Task<object> GetCurrentOrganizationAsync();
        Task<object> GetOrganizationMembersAsync();
        Task InviteUserAsync(InviteUserDto dto);
        Task RemoveUserAsync(Guid userId);
        Task<OrganizationDetailDto> GetOrganizationDetailAsync();
        Task<OrganizationDetailDto> UpdateOrganizationAsync(UpdateOrganizationDto dto);
    }
}