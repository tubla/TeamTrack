namespace TeamTrack.Api.Models.Rbac.Constants
{
    public static class PermissionConstants
    {
        // Organization
        public const string CreateOrganization = "org.create";
        public const string ViewOrganization = "org.view";
        public const string UpdateOrganization = "org.update";
        public const string DeleteOrganization = "org.delete";
        public const string ManageMembers = "org.members.manage";

        // Project
        public const string CreateProject = "project.create";
        public const string ViewProject = "project.view";
        public const string UpdateProject = "project.update";
        public const string DeleteProject = "project.delete";

        // Task
        public const string CreateTask = "task.create";
        public const string ViewTask = "task.view";
        public const string UpdateTask = "task.update";
        public const string DeleteTask = "task.delete";
        public const string AssignTask = "task.assign";

        // Role
        public const string CreateRole = "role.create";
        public const string ViewRole = "role.view";
        public const string UpdateRole = "role.update";
        public const string DeleteRole = "role.delete";
        public const string AssignRole = "role.assign";

        // Permission
        public const string ViewPermission = "permission.view";

        // User
        public const string ViewUser = "user.view";
        public const string UpdateUser = "user.update";
        public const string AssignUserRole = "user.role.assign";

        // Comment
        public const string CreateComment = "comment.create";
        public const string ViewComment = "comment.view";
        public const string DeleteComment = "comment.delete";

        // Notification
        public const string ViewNotification = "notification.view";
        public const string UpdateNotification = "notification.update";

        // Attachment
        public const string ViewAttachment = "attachment.view";
        public const string UploadAttachment = "attachment.upload";
        public const string DeleteAttachment = "attachment.delete";

        // Dashboard
        public const string ViewDashboard = "dashboard.view";

        // Additional Permissions
        public const string OrganizationNotificationAccess = "org.notification";
        public const string OrganizationAttachmentAccess = "org.attachment";

        // New Permissions for Notification and Attachment related to Organization
        public const string ManageNotificationOrganization = "notification.org.manage";
        public const string ManageAttachmentOrganization = "attachment.org.manage";
    }
}
