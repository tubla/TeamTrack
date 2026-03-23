namespace TeamTrack.Api.Models.Rbac;

public class Role : BaseEntity
{
    public required string Name { get; set; }

    // Multi-tenant scope
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    // Navigation
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
