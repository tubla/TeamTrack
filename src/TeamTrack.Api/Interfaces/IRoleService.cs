using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Permission;
using TeamTrack.Api.DTOs.Role;

namespace TeamTrack.Api.Interfaces
{
    public interface IRoleService
    {
        Task<object> CreateRoleAsync(CreateRoleDto dto);
        Task AssignPermissionsAsync(AssignPermissionsDto dto);
        Task AssignRoleToUserAsync(AssignRoleToUserDto dto);
        Task RemoveRoleFromUserAsync(Guid userId, Guid roleId);
        Task<ApiResponse<List<RoleDto>>> GetRolesAsync();
        Task<object> GetRoleWithPermissionsAsync(Guid roleId);
    }
}
