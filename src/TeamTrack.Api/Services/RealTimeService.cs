using Microsoft.AspNetCore.SignalR;
using TeamTrack.Api.DTOs.Member;
using TeamTrack.Api.DTOs.Project;
using TeamTrack.Api.DTOs.SignalR;
using TeamTrack.Api.DTOs.Task;
using TeamTrack.Api.Hubs;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services;

public class RealTimeService : IRealTimeService
{
    private readonly IHubContext<TeamTrackHub> _hubContext;
    private readonly IRequestContext _requestContext;

    public RealTimeService(IHubContext<TeamTrackHub> hubContext, IRequestContext requestContext)
    {
        _hubContext = hubContext;
        _requestContext = requestContext;
    }

    // Task Events
    public async Task NotifyTaskCreated(Guid organizationId, Guid projectId, TaskDto task)
    {
        var eventData = new TaskEventDto
        {
            EventType = "Created",
            TaskId = task.Id,
            ProjectId = projectId,
            Task = task,
            TriggeredBy = _requestContext.Email
        };

        await _hubContext.Clients.Group($"project_{projectId}").SendAsync("TaskCreated", eventData);
        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("TaskEvent", eventData);
    }

    public async Task NotifyTaskUpdated(Guid organizationId, Guid projectId, TaskDto task)
    {
        var eventData = new TaskEventDto
        {
            EventType = "Updated",
            TaskId = task.Id,
            ProjectId = projectId,
            Task = task,
            TriggeredBy = _requestContext.Email
        };

        await _hubContext.Clients.Group($"project_{projectId}").SendAsync("TaskUpdated", eventData);
    }

    public async Task NotifyTaskDeleted(Guid organizationId, Guid projectId, Guid taskId)
    {
        var eventData = new TaskEventDto
        {
            EventType = "Deleted",
            TaskId = taskId,
            ProjectId = projectId,
            TriggeredBy = _requestContext.Email
        };

        await _hubContext.Clients.Group($"project_{projectId}").SendAsync("TaskDeleted", eventData);
    }

    public async Task NotifyTaskStatusChanged(Guid organizationId, Guid projectId, Guid taskId, string oldStatus, string newStatus)
    {
        var eventData = new TaskEventDto
        {
            EventType = "StatusChanged",
            TaskId = taskId,
            ProjectId = projectId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            TriggeredBy = _requestContext.Email
        };

        await _hubContext.Clients.Group($"project_{projectId}").SendAsync("TaskStatusChanged", eventData);
    }

    // Project Events
    public async Task NotifyProjectCreated(Guid organizationId, ProjectDto project)
    {
        var eventData = new ProjectEventDto
        {
            EventType = "Created",
            ProjectId = project.Id,
            Project = project
        };

        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("ProjectCreated", eventData);
    }

    public async Task NotifyProjectUpdated(Guid organizationId, ProjectDto project)
    {
        var eventData = new ProjectEventDto
        {
            EventType = "Updated",
            ProjectId = project.Id,
            Project = project
        };

        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("ProjectUpdated", eventData);
    }

    public async Task NotifyProjectDeleted(Guid organizationId, Guid projectId)
    {
        var eventData = new ProjectEventDto
        {
            EventType = "Deleted",
            ProjectId = projectId
        };

        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("ProjectDeleted", eventData);
    }

    // Member Events
    public async Task NotifyMemberJoined(Guid organizationId, MemberDto member)
    {
        var eventData = new MemberEventDto
        {
            EventType = "Joined",
            UserId = member.UserId,
            Member = member
        };

        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("MemberJoined", eventData);
    }

    public async Task NotifyMemberRemoved(Guid organizationId, Guid userId)
    {
        var eventData = new MemberEventDto
        {
            EventType = "Removed",
            UserId = userId
        };

        await _hubContext.Clients.Group($"org_{organizationId}").SendAsync("MemberRemoved", eventData);
    }

    // User Notifications
    public async Task NotifyUser(Guid userId, NotificationDto notification)
    {
        await _hubContext.Clients.Group($"user_{userId}").SendAsync("Notification", notification);
    }

    public async Task NotifyTaskAssigned(Guid userId, TaskDto task)
    {
        var notification = new NotificationDto
        {
            Type = "TaskAssigned",
            Title = "New Task Assigned",
            Message = $"You've been assigned to: {task.Title}",
            Link = $"/projects/{task.ProjectId}/board"
        };

        await NotifyUser(userId, notification);
    }
}