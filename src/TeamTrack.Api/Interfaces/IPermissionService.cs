namespace TeamTrack.Api.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(Guid userId, string permission);
    }
}
