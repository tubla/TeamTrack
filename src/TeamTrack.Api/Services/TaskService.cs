using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;
using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Extensions;

namespace TeamTrack.Api.Services
{
    public class TaskService(ApplicationDbContext db,INotificationService notificationService, IRequestContext context) : ITaskService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<object> CreateAsync(CreateTaskDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var projectExists = await _db.Projects
                .AnyAsync(p => p.Id == dto.ProjectId && p.OrganizationId == orgId);

            if (!projectExists)
                throw new BadRequestException("Invalid project");

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

            // Notify assigned user if applicable (not implemented here)
            if (dto.AssignedToUserId.HasValue)
            {
                await _notificationService.CreateAsync(
                    dto.AssignedToUserId.Value,
                    "Task Assigned",
                    $"You have been assigned task: {dto.Title}",
                    NotificationType.TaskAssigned,
                    task.Id
                );
            }

            return new { task.Id, task.Title };
        }

        public async Task<object> GetAsync(QueryParams param, Guid projectId)
        {
            var query = _db.Tasks
                .Where(t => t.ProjectId == projectId);

            var total = await query.CountAsync();

            var items = await query
                .ApplyPaging(param)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Status,
                    t.Priority
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

        public async Task UpdateStatusAsync(UpdateTaskStatusDto dto)
        {
            var task = await _db.Tasks.FindAsync(dto.TaskId);

            if (task == null)
                throw new NotFoundException("Task not found");

            task.Status = dto.Status;

            await _db.SaveChangesAsync();
        }
    }
}
