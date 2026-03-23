using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Models;
using TeamTrack.Api.Models.Rbac;
using TaskStatus = TeamTrack.Api.Models.TaskStatus;

namespace TeamTrack.Api.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Only seed if database is empty
            if (await context.Users.AnyAsync())
                return;

            // ========== 1. SEED PERMISSIONS ==========
            var permissions = new List<Permission>
            {
                // Organization
                new() { Name = "org.create" },
                new() { Name = "org.view" },
                new() { Name = "org.update" },
                new() { Name = "org.delete" },
                new() { Name = "org.members.manage" },
                new() { Name = "org.notification" },
                new() { Name = "org.attachment" },

                // Project
                new() { Name = "project.create" },
                new() { Name = "project.view" },
                new() { Name = "project.update" },
                new() { Name = "project.delete" },

                // Task
                new() { Name = "task.create" },
                new() { Name = "task.view" },
                new() { Name = "task.update" },
                new() { Name = "task.delete" },
                new() { Name = "task.assign" },

                // Role
                new() { Name = "role.create" },
                new() { Name = "role.view" },
                new() { Name = "role.update" },
                new() { Name = "role.delete" },
                new() { Name = "role.assign" },

                // Permission
                new() { Name = "permission.view" },

                // User
                new() { Name = "user.view" },
                new() { Name = "user.update" },
                new() { Name = "user.role.assign" },

                // Comment
                new() { Name = "comment.create" },
                new() { Name = "comment.view" },
                new() { Name = "comment.delete" },

                // Notification
                new() { Name = "notification.view" },
                new() { Name = "notification.update" },
                new() { Name = "notification.org.manage" },

                // Attachment
                new() { Name = "attachment.view" },
                new() { Name = "attachment.upload" },
                new() { Name = "attachment.delete" },
                new() { Name = "attachment.org.manage" },

                // Dashboard
                new() { Name = "dashboard.view" }
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();

            // ========== 2. SEED ADMIN USER ==========
            var adminUser = new User
            {
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@teamtrack.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                IsActive = true
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            // ========== 3. SEED DEFAULT ORGANIZATION ==========
            var defaultOrg = new Organization
            {
                Name = "TeamTrack Demo",
                OwnerUserId = adminUser.Id
            };

            context.Organizations.Add(defaultOrg);
            
            // Add admin to organization
            context.OrganizationUsers.Add(new OrganizationUser
            {
                UserId = adminUser.Id,
                OrganizationId = defaultOrg.Id
            });

            // Update admin's last active org
            adminUser.LastActiveOrganizationId = defaultOrg.Id;

            await context.SaveChangesAsync();

            // ========== 4. SEED ADMIN ROLE ==========
            var adminRole = new Role
            {
                Name = "Admin",
                OrganizationId = defaultOrg.Id
            };

            var memberRole = new Role
            {
                Name = "Member",
                OrganizationId = defaultOrg.Id
            };

            context.Roles.AddRange(adminRole, memberRole);
            await context.SaveChangesAsync();

            // ========== 5. ASSIGN ALL PERMISSIONS TO ADMIN ROLE ==========
            var allPermissions = await context.Permissions.ToListAsync();
            
            var adminRolePermissions = allPermissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id
            });

            context.RolePermissions.AddRange(adminRolePermissions);

            // Member role gets limited permissions
            var memberPermissionNames = new[]
            {
                "org.view", "project.view", "task.create", "task.view", "task.update",
                "comment.create", "comment.view", "notification.view", "notification.update",
                "attachment.view", "attachment.upload", "dashboard.view"
            };

            var memberPermissions = allPermissions
                .Where(p => memberPermissionNames.Contains(p.Name))
                .Select(p => new RolePermission
                {
                    RoleId = memberRole.Id,
                    PermissionId = p.Id
                });

            context.RolePermissions.AddRange(memberPermissions);
            await context.SaveChangesAsync();

            // ========== 6. ASSIGN ADMIN ROLE TO ADMIN USER ==========
            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id,
                OrganizationId = defaultOrg.Id
            });

            await context.SaveChangesAsync();

            // ========== 7. SEED DEMO PROJECT ==========
            var demoProject = new Project
            {
                Name = "Demo Project",
                Description = "A sample project to get started",
                OrganizationId = defaultOrg.Id,
                CreatedByUserId = adminUser.Id
            };

            context.Projects.Add(demoProject);
            await context.SaveChangesAsync();

            // ========== 8. SEED DEMO TASKS ==========
            var demoTasks = new List<TaskItem>
            {
                new()
                {
                    Title = "Setup development environment",
                    Description = "Install required tools and configure the project",
                    ProjectId = demoProject.Id,
                    CreatedByUserId = adminUser.Id,
                    AssignedToUserId = adminUser.Id,
                    Priority = TaskPriority.High,
                    Status = TaskStatus.Done,
                    DueDate = DateTimeOffset.UtcNow.AddDays(-2)
                },
                new()
                {
                    Title = "Review API documentation",
                    Description = "Go through the API endpoints and understand the flow",
                    ProjectId = demoProject.Id,
                    CreatedByUserId = adminUser.Id,
                    AssignedToUserId = adminUser.Id,
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.InProgress,
                    DueDate = DateTimeOffset.UtcNow.AddDays(3)
                },
                new()
                {
                    Title = "Create UI mockups",
                    Description = "Design the user interface for the main features",
                    ProjectId = demoProject.Id,
                    CreatedByUserId = adminUser.Id,
                    Priority = TaskPriority.Medium,
                    Status = TaskStatus.Todo,
                    DueDate = DateTimeOffset.UtcNow.AddDays(7)
                },
                new()
                {
                    Title = "Implement authentication",
                    Description = "Add login, register, and JWT token handling",
                    ProjectId = demoProject.Id,
                    CreatedByUserId = adminUser.Id,
                    Priority = TaskPriority.High,
                    Status = TaskStatus.Todo,
                    DueDate = DateTimeOffset.UtcNow.AddDays(5)
                }
            };

            context.Tasks.AddRange(demoTasks);
            await context.SaveChangesAsync();

            // ========== 9. SEED DEMO MEMBER USER ==========
            var memberUser = new User
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@teamtrack.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member@123"),
                IsActive = true,
                LastActiveOrganizationId = defaultOrg.Id
            };

            context.Users.Add(memberUser);
            await context.SaveChangesAsync();

            // Add member to organization
            context.OrganizationUsers.Add(new OrganizationUser
            {
                UserId = memberUser.Id,
                OrganizationId = defaultOrg.Id
            });

            // Assign member role
            context.UserRoles.Add(new UserRole
            {
                UserId = memberUser.Id,
                RoleId = memberRole.Id,
                OrganizationId = defaultOrg.Id
            });

            await context.SaveChangesAsync();

            // ========== 10. SEED WELCOME NOTIFICATION ==========
            context.Notifications.Add(new Notification
            {
                UserId = adminUser.Id,
                OrganizationId = defaultOrg.Id,
                Title = "Welcome to TeamTrack!",
                Message = "Your account has been set up successfully. Start by exploring the demo project.",
                Type = NotificationType.General,
                IsRead = false
            });

            context.Notifications.Add(new Notification
            {
                UserId = memberUser.Id,
                OrganizationId = defaultOrg.Id,
                Title = "Welcome to TeamTrack!",
                Message = "You've been added to TeamTrack Demo organization.",
                Type = NotificationType.General,
                IsRead = false
            });

            await context.SaveChangesAsync();
        }
    }
}