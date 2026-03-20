namespace TeamTrack.Api.Models
{
    public class TaskComment : BaseEntity
    {
        public Guid TaskId { get; set; }
        public TaskItem Task { get; set; } = default!;

        public Guid UserId { get; set; }

        public string Content { get; set; } = default!;
    }
}
