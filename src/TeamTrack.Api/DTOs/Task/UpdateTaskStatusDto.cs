namespace TeamTrack.Api.DTOs.Task;

public class UpdateTaskStatusDto
{
    public Guid TaskId { get; set; }
    public Models.TaskStatus Status { get; set; }
}
