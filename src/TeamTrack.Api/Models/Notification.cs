namespace TeamTrack.Api.Models;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public required string Title { get; set; }
    public required string Message { get; set; }

    public bool IsRead { get; set; } = false;

    public NotificationType Type { get; set; }

    public Guid? ReferenceId { get; set; }
}