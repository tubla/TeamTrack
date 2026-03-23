using TeamTrack.Api.DTOs.Project;
namespace TeamTrack.Api.DTOs.Dashboard;

public class DashboardDto
{
    public DashboardStatsDto Stats { get; set; } = new();
    public List<RecentTaskDto> RecentTasks { get; set; } = [];
    public List<ProjectProgressDto> ProjectProgress { get; set; } = [];
    public TaskDistributionDto TaskDistribution { get; set; } = new();
}
