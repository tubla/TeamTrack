using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
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
            var comment = new TaskComment
            {
                TaskId = dto.TaskId,
                Content = dto.Content,
                UserId = _context.UserId
            };

            _db.TaskComments.Add(comment);

            // 🔥 Add Activity Log
            _db.ActivityLogs.Add(new ActivityLog
            {
                OrganizationId = _context.OrganizationId!.Value,
                UserId = _context.UserId,
                Action = "CommentAdded",
                EntityType = "Task",
                EntityId = dto.TaskId,
                Metadata = dto.Content
            });

            await _db.SaveChangesAsync();

            // Notify task owner about the new comment
            await _notificationService.CreateAsync(
                _context.UserId,
                "New Comment",
                "Someone commented on your task",
                NotificationType.CommentAdded,
                dto.TaskId
            );

            return new { comment.Id, comment.Content };
        }

        public async Task<object> GetByTaskAsync(Guid taskId)
        {
            var comments = await _db.TaskComments
                .Where(c => c.TaskId == taskId)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt
                }).ToListAsync();

            return comments;
        }
    }
}