namespace TeamTrack.Api.Models
{
    public enum TaskStatus
    {
        Todo,
        InProgress,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public enum NotificationType
    {
        General,
        TaskAssigned,
        CommentAdded,
        StatusChanged,
        DueDateReminder,
        OrgAccessRequested,
        OrgAccessApproved,
        OrgAccessRejected
    }
}
