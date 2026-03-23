using TeamTrack.Api.Models;

namespace TeamTrack.Api.DTOs.Task;

// ===== Task DTOs =====
public class CreateTaskDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
}
