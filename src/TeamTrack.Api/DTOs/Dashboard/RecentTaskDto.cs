namespace TeamTrack.Api.DTOs.Dashboard;

public class RecentTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
