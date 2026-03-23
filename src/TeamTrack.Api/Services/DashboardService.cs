using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Dashboard;
using TeamTrack.Api.DTOs.Project;
using TeamTrack.Api.Interfaces;
using TaskStatus = TeamTrack.Api.Models.TaskStatus;

namespace TeamTrack.Api.Services;

public class DashboardService(ApplicationDbContext db, IRequestContext context) : IDashboardService
{
    private readonly ApplicationDbContext _db = db;
    private readonly IRequestContext _context = context;

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var orgId = _context.OrganizationId;
        var userId = _context.UserId;

        if (orgId == null)
            return new DashboardDto();

        var projects = await _db.Projects
            .Where(p => p.OrganizationId == orgId)
            .Select(p => new
            {
                p.Id,
                p.Name,
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == TaskStatus.Done)
            })
            .ToListAsync();

        var tasks = await _db.Tasks
            .Where(t => t.Project.OrganizationId == orgId)
            .ToListAsync();

        var memberCount = await _db.OrganizationUsers
            .CountAsync(ou => ou.OrganizationId == orgId);

        var stats = new DashboardStatsDto
        {
            TotalProjects = projects.Count,
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Done),
            MyTasks = tasks.Count(t => t.AssignedToUserId == userId),
            OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date < DateTimeOffset.UtcNow.Date && t.Status != TaskStatus.Done),
            TeamMembers = memberCount
        };

        var recentTasks = await _db.Tasks
            .Where(t => t.Project.OrganizationId == orgId)
            .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
            .Take(5)
            .Select(t => new RecentTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                ProjectName = t.Project.Name,
                ProjectId = t.ProjectId,
                DueDate = t.DueDate,
                UpdatedAt = t.UpdatedAt ?? t.CreatedAt
            })
            .ToListAsync();

        var projectProgress = projects
            .OrderByDescending(p => p.TotalTasks)
            .Take(5)
            .Select(p => new ProjectProgressDto
            {
                Id = p.Id,
                Name = p.Name,
                TotalTasks = p.TotalTasks,
                CompletedTasks = p.CompletedTasks
            })
            .ToList();

        var taskDistribution = new TaskDistributionDto
        {
            Todo = tasks.Count(t => t.Status == TaskStatus.Todo),
            InProgress = tasks.Count(t => t.Status == TaskStatus.InProgress),
            Review = 0,
            Done = tasks.Count(t => t.Status == TaskStatus.Done)
        };

        return new DashboardDto
        {
            Stats = stats,
            RecentTasks = recentTasks,
            ProjectProgress = projectProgress,
            TaskDistribution = taskDistribution
        };
    }
}