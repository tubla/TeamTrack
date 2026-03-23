using TeamTrack.Api.DTOs.Member;
using TeamTrack.Api.DTOs.Project;
using TeamTrack.Api.DTOs.SignalR;
using TeamTrack.Api.DTOs.Task;

namespace TeamTrack.Api.Interfaces
{
    public interface IRealTimeService
    {
        // Task Events
        Task NotifyTaskCreated(Guid organizationId, Guid projectId, TaskDto task);
        Task NotifyTaskUpdated(Guid organizationId, Guid projectId, TaskDto task);
        Task NotifyTaskDeleted(Guid organizationId, Guid projectId, Guid taskId);
        Task NotifyTaskStatusChanged(Guid organizationId, Guid projectId, Guid taskId, string oldStatus, string newStatus);

        // Project Events
        Task NotifyProjectCreated(Guid organizationId, ProjectDto project);
        Task NotifyProjectUpdated(Guid organizationId, ProjectDto project);
        Task NotifyProjectDeleted(Guid organizationId, Guid projectId);

        // Member Events
        Task NotifyMemberJoined(Guid organizationId, MemberDto member);
        Task NotifyMemberRemoved(Guid organizationId, Guid userId);

        // User Notifications
        Task NotifyUser(Guid userId, NotificationDto notification);
        Task NotifyTaskAssigned(Guid userId, TaskDto task);
    }
}
