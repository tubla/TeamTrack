using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Extensions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class NotificationService(ApplicationDbContext db, IRequestContext context) : INotificationService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;

        public async Task CreateAsync(Guid userId, Guid? organizationId, string title, string message, NotificationType type, Guid? referenceId)
        {
            var notification = new Notification
            {
                UserId = userId,
                OrganizationId = organizationId,
                Title = title,
                Message = message,
                Type = type,
                ReferenceId = referenceId
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }


        public async Task CreateAsync(string title, string message, NotificationType type, Guid? referenceId)
        {
            var adminUsers = await _db.UserRoles
                .Where(ur => ur.Role.Name == "Admin" || ur.Role.Name == "SuperAdmin")
                .Select(ur => ur.User)
                .ToListAsync();

            if (adminUsers.Count == 0)
                return;

            foreach (var admin in adminUsers)
            {
                // Create notification DB record
                var notification = new Notification
                {
                    UserId = admin.Id,
                    OrganizationId = null,  // This is a system-level notification
                    Title = title,
                    Message = message,
                    Type = type,
                    ReferenceId = referenceId
                };

                _db.Notifications.Add(notification);
            }
            await _db.SaveChangesAsync();
        }


        public async Task<object> GetMyAsync(QueryParams param)
        {
            var userId = _context.UserId;
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var query = _db.Notifications
                .Where(n => n.UserId == userId && n.OrganizationId == orgId);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedAt)
                .ApplyPaging(param)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.IsRead,
                    n.Type,
                    n.ReferenceId,
                    n.CreatedAt
                }).ToListAsync();

            return new PagedResponse<object>
            {
                Items = items,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }

        public async Task<int> GetUnreadCountAsync()
        {
            var userId = _context.UserId;
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            return await _db.Notifications
                .CountAsync(n => n.UserId == userId && n.OrganizationId == orgId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var userId = _context.UserId;
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId && n.OrganizationId == orgId);

            if (notification == null)
                throw new NotFoundException("Notification not found");

            notification.IsRead = true;
            await _db.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync()
        {
            var userId = _context.UserId;
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var unreadNotifications = await _db.Notifications
                .Where(n => n.UserId == userId && n.OrganizationId == orgId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _db.SaveChangesAsync();
        }
    }
}