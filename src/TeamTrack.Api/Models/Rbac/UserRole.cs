namespace TeamTrack.Api.Models.Rbac;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // Multi-tenant scope
    public Guid OrganizationId { get; set; }
}
