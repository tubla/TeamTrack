namespace TeamTrack.Api.Models.Rbac
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; } = default!;

        // Navigation
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
