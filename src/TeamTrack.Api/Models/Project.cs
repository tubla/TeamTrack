namespace TeamTrack.Api.Models
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }

        // Navigation
        public ICollection<TaskItem> Tasks { get; set; } = [];
    }
}
