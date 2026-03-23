namespace TeamTrack.Api.DTOs.Member
{
    public class MemberDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? AvatarUrl { get; set; }
        public List<MemberRoleDto> Roles { get; set; } = [];
        public DateTimeOffset JoinedAt { get; set; }
        public bool IsOwner { get; set; }
    }
}
