using TeamTrack.Api.Models.Rbac;

namespace TeamTrack.Api.Models;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }
    public Guid? LastActiveOrganizationId { get; set; }

    // Navigation Properties
    public ICollection<OrganizationUser> OrganizationUsers { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}