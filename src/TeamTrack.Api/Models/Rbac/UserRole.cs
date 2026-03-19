namespace TeamTrack.Api.Models.Rbac
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = default!;

        // 🔥 CRITICAL for multi-tenant
        public Guid OrganizationId { get; set; }
    }
}
