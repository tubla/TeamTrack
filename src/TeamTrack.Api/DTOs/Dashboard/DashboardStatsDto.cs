namespace TeamTrack.Api.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int MyTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int TeamMembers { get; set; }
    public int UnreadNotifications { get; set; }
}
