namespace TeamTrack.Api.Models;

public class TaskItem : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? AssignedToUserId { get; set; }

    public Guid CreatedByUserId { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTimeOffset? DueDate { get; set; }
}
