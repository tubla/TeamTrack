namespace TeamTrack.Api.Models;

public class Project : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    // Navigation
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
