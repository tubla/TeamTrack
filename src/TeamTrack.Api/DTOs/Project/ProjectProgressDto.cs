namespace TeamTrack.Api.DTOs.Project;

public class ProjectProgressDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double ProgressPercent => TotalTasks == 0 ? 0 : Math.Round((double)CompletedTasks / TotalTasks * 100, 1);
}
