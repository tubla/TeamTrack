using TeamTrack.Api.Models.Rbac;

namespace TeamTrack.Api.Models
{
    public class User : BaseEntity
    {
        // Basic Info
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Auth
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Status
        public bool IsActive { get; set; } = true;

        // Optional (good for future)
        public DateTime? LastLoginAt { get; set; }

        // Navigation Properties

        // Organizations user belongs to
        public ICollection<OrganizationUser> OrganizationUsers { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}