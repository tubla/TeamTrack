using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Comment;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class CommentService(ApplicationDbContext db, INotificationService notificationService, IRequestContext context) : ICommentService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<object> CreateAsync(CreateCommentDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            // Validate task belongs to organization
            var task = await _db.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == dto.TaskId && t.Project.OrganizationId == orgId);

            if (task == null)
                throw new NotFoundException("Task not found");

            var comment = new TaskComment
            {
                TaskId = dto.TaskId,
                Content = dto.Content,
                UserId = _context.UserId
            };

            _db.TaskComments.Add(comment);

            // Add Activity Log
            _db.ActivityLogs.Add(new ActivityLog
            {
                OrganizationId = orgId,
                UserId = _context.UserId,
                Action = "CommentAdded",
                EntityType = "Task",
                EntityId = dto.TaskId,
                Metadata = dto.Content
            });

            await _db.SaveChangesAsync();

            // Notify task creator/assignee about the new comment
            if (task.CreatedByUserId != _context.UserId)
            {
                await _notificationService.CreateAsync(
                    task.CreatedByUserId,
                    orgId,
                    "New Comment",
                    $"Someone commented on task: {task.Title}",
                    NotificationType.CommentAdded,
                    dto.TaskId
                );
            }

            if (task.AssignedToUserId.HasValue && task.AssignedToUserId != _context.UserId && task.AssignedToUserId != task.CreatedByUserId)
            {
                await _notificationService.CreateAsync(
                    task.AssignedToUserId.Value,
                    orgId,
                    "New Comment",
                    $"Someone commented on task: {task.Title}",
                    NotificationType.CommentAdded,
                    dto.TaskId
                );
            }

            return new { comment.Id, comment.Content };
        }

        public async Task<object> GetByTaskAsync(Guid taskId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            // Validate task belongs to organization
            var taskExists = await _db.Tasks
                .Include(t => t.Project)
                .AnyAsync(t => t.Id == taskId && t.Project.OrganizationId == orgId);

            if (!taskExists)
                throw new NotFoundException("Task not found");

            var comments = await _db.TaskComments
                .Where(c => c.TaskId == taskId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    c.UserId
                }).ToListAsync();

            return comments;
        }
    }
}