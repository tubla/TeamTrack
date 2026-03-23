using TeamTrack.Api.DTOs.Permission;

namespace TeamTrack.Api.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(Guid userId, string permission);
        Task<object> GetAllAsync();
        List<PermissionGroupDto> GetAllGroupedPermissions();
    }
}
