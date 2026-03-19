namespace TeamTrack.Api.Models.Rbac
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = default!;

        // Multi-tenant scope
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
