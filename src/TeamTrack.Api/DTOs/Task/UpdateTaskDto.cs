using TeamTrack.Api.Models;

namespace TeamTrack.Api.DTOs.Task;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
