using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Permission;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Services
{
    public class PermissionService(ApplicationDbContext db, IRequestContext context) : IPermissionService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;

        public async Task<bool> HasPermission(Guid userId, string permission)
        {
            var orgId = _context.OrganizationId;

            return await _db.UserRoles
                .Where(ur => ur.UserId == userId && ur.OrganizationId == orgId)
                .Join(_db.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_db.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .AnyAsync(p => p.Name == permission);
        }

        public async Task<object> GetAllAsync()
        {
            var permissions = await _db.Permissions
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToListAsync();

            return permissions;
        }

        public List<PermissionGroupDto> GetAllGroupedPermissions()
        {
            return
            [
                new PermissionGroupDto
            {
                GroupName = "Organization",
                Icon = "Business",
                Permissions =
                [
                    new PermissionDto
                    {
                        Key = PermissionConstants.ViewOrganization,
                        Name = "View Organization",
                        Description = "View organization details and settings"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.CreateOrganization,
                        Name = "Manage Organization",
                        Description = "Create, update organization and invite members"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.UpdateOrganization,
                        Name = "Update Organization",
                        Description = "Update organization settings"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.DeleteOrganization,
                        Name = "Delete Organization",
                        Description = "Delete the organization"
                    }
                ]
            },
            new PermissionGroupDto
            {
                GroupName = "Project",
                Icon = "Folder",
                Permissions =
                [
                    new PermissionDto
                    {
                        Key = PermissionConstants.ViewProject,
                        Name = "View Projects",
                        Description = "View projects and their details"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.CreateProject,
                        Name = "Create Projects",
                        Description = "Create new projects"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.UpdateProject,
                        Name = "Update Projects",
                        Description = "Update project details"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.DeleteProject,
                        Name = "Delete Projects",
                        Description = "Delete projects"
                    }
                ]
            },
            new PermissionGroupDto
            {
                GroupName = "Task",
                Icon = "Task",
                Permissions =
                [
                    new PermissionDto
                    {
                        Key = PermissionConstants.ViewTask,
                        Name = "View Tasks",
                        Description = "View tasks and their details"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.CreateTask,
                        Name = "Create Tasks",
                        Description = "Create new tasks"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.UpdateTask,
                        Name = "Update Tasks",
                        Description = "Update task details and status"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.DeleteTask,
                        Name = "Delete Tasks",
                        Description = "Delete tasks"
                    }
                ]
            },
            new PermissionGroupDto
            {
                GroupName = "Role & Access",
                Icon = "Security",
                Permissions =
                [
                    new PermissionDto
                    {
                        Key = PermissionConstants.ViewRole,
                        Name = "View Roles",
                        Description = "View roles and permissions"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.CreateRole,
                        Name = "Manage Roles",
                        Description = "Create and update roles"
                    },
                    new PermissionDto
                    {
                        Key = PermissionConstants.AssignRole,
                        Name = "Assign Roles",
                        Description = "Assign roles to users"
                    }
                ]
            }
            ];
        }
    }
}