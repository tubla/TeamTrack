namespace TeamTrack.Api.Models.Rbac;

public class Permission : BaseEntity
{
    public required string Name { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
