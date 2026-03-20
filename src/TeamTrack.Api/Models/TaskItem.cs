namespace TeamTrack.Api.Models
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = default!;

        public Guid? AssignedToUserId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime? DueDate { get; set; }
    }
}
