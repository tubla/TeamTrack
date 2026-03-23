using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;
using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Extensions;
using TeamTrack.Api.DTOs.Task;

namespace TeamTrack.Api.Services
{
    public class TaskService(ApplicationDbContext db, INotificationService notificationService, IRequestContext context, IRealTimeService realTimeService) : ITaskService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IRealTimeService _realTimeService = realTimeService;

        public async Task<object> CreateAsync(CreateTaskDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var projectExists = await _db.Projects
                .AnyAsync(p => p.Id == dto.ProjectId && p.OrganizationId == orgId);

            if (!projectExists)
                throw new BadRequestException("Invalid project");

            if (dto.AssignedToUserId.HasValue)
            {
                var isOrgMember = await _db.OrganizationUsers
                    .AnyAsync(ou => ou.UserId == dto.AssignedToUserId.Value && ou.OrganizationId == orgId);

                if (!isOrgMember)
                    throw new BadRequestException("Assigned user is not a member of this organization");
            }

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedToUserId = dto.AssignedToUserId,
                CreatedByUserId = _context.UserId,
                Priority = dto.Priority,
                DueDate = dto.DueDate
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            var taskDto = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                AssigneeId = task.AssignedToUserId,
                ProjectId = task.ProjectId
            };

            // Emit real-time event
            await _realTimeService.NotifyTaskCreated(orgId, dto.ProjectId, taskDto);

            // Notify assignee if assigned
            if (dto.AssignedToUserId.HasValue)
            {
                await _realTimeService.NotifyTaskAssigned(dto.AssignedToUserId.Value, taskDto);
            }

            return taskDto;
        }

        public async Task<object> GetAsync(QueryParams param, Guid projectId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var projectExists = await _db.Projects
                .AnyAsync(p => p.Id == projectId && p.OrganizationId == orgId);

            if (!projectExists)
                throw new NotFoundException("Project not found");

            var query = _db.Tasks.Where(t => t.ProjectId == projectId);

            var total = await query.CountAsync();

            var items = await query
                .ApplyPaging(param)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.AssignedToUserId
                })
                .ToListAsync();

            return new PagedResponse<object>
            {
                Items = items,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }

        public async Task<object> GetByIdAsync(Guid id)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var task = await _db.Tasks
                .Include(t => t.Project)
                .Where(t => t.Id == id && t.Project.OrganizationId == orgId)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    t.AssignedToUserId,
                    t.CreatedByUserId,
                    t.ProjectId,
                    ProjectName = t.Project.Name,
                    t.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
                throw new NotFoundException("Task not found");

            return task;
        }

        public async Task<object> GetMyTasksAsync(QueryParams param)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");
            var userId = _context.UserId;

            var query = _db.Tasks
                .Include(t => t.Project)
                .Where(t => t.Project.OrganizationId == orgId && t.AssignedToUserId == userId);

            var total = await query.CountAsync();

            var items = await query
                .ApplyPaging(param)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Status,
                    t.Priority,
                    t.DueDate,
                    ProjectName = t.Project.Name
                })
                .ToListAsync();

            return new PagedResponse<object>
            {
                Items = items,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }

        public async Task<object> UpdateAsync(Guid id, UpdateTaskDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var task = await _db.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == orgId);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;

            if (dto.Description != null)
                task.Description = dto.Description;

            if (dto.Priority.HasValue)
                task.Priority = dto.Priority.Value;

            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate.Value;

            await _db.SaveChangesAsync();

            return new { task.Id, task.Title };
        }

        public async Task UpdateStatusAsync(UpdateTaskStatusDto dto)
        {
            var task = await _db.Tasks.FindAsync(dto.TaskId);
            if (task == null) throw new KeyNotFoundException("Task not found");

            var oldStatus = task.Status;
            task.Status = dto.Status;
            task.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            // Emit real-time event
            var orgId = _context.OrganizationId!.Value;
            await _realTimeService.NotifyTaskStatusChanged(
                orgId,
                task.ProjectId,
                task.Id,
                oldStatus.ToString(),
                dto.Status.ToString()
            );
        }

        public async Task AssignTaskAsync(Guid id, AssignTaskDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var task = await _db.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == orgId);

            if (task == null)
                throw new NotFoundException("Task not found");

            if (dto.AssignedToUserId.HasValue)
            {
                var isOrgMember = await _db.OrganizationUsers
                    .AnyAsync(ou => ou.UserId == dto.AssignedToUserId.Value && ou.OrganizationId == orgId);

                if (!isOrgMember)
                    throw new BadRequestException("Assigned user is not a member of this organization");

                await _notificationService.CreateAsync(
                    dto.AssignedToUserId.Value,
                    orgId,
                    "Task Assigned",
                    $"You have been assigned task: {task.Title}",
                    NotificationType.TaskAssigned,
                    task.Id
                );
            }

            task.AssignedToUserId = dto.AssignedToUserId;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var task = await _db.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.OrganizationId == orgId);

            if (task == null)
                throw new NotFoundException("Task not found");

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }
    }
}