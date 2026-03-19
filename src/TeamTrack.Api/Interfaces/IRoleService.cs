using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface IRoleService
    {
        Task<object> CreateRoleAsync(CreateRoleDto dto);
        Task AssignPermissionsAsync(AssignPermissionsDto dto);
        Task AssignRoleToUserAsync(AssignRoleToUserDto dto);
        Task<object> GetRolesAsync(QueryParams param);
    }
}
