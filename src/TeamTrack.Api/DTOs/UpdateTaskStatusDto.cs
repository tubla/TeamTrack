namespace TeamTrack.Api.DTOs
{
    public class UpdateTaskStatusDto
    {
        public Guid TaskId { get; set; }
        public Models.TaskStatus Status { get; set; }
    }
}
