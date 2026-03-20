using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Extensions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

public class NotificationService(ApplicationDbContext db, IRequestContext context) : INotificationService
{
    private readonly ApplicationDbContext _db = db;
    private readonly IRequestContext _context = context;

    public async Task CreateAsync(Guid userId, string title, string message, NotificationType type, Guid? referenceId)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ReferenceId = referenceId
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<object> GetMyAsync(QueryParams param)
    {
        var userId = _context.UserId;

        var query = _db.Notifications
            .Where(n => n.UserId == userId);

        var total = await EntityFrameworkQueryableExtensions.CountAsync(query);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .ApplyPaging(param)
            .Select(n => new
            {
                n.Id,
                n.Title,
                n.Message,
                n.IsRead
            }).ToListAsync();

        return new PagedResponse<object>
        {
            Items = items,
            Page = param.Page,
            PageSize = param.PageSize,
            TotalCount = total
        };
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _db.Notifications.FindAsync(notificationId);

        if (notification == null)
            throw new NotFoundException("Notification not found");

        notification.IsRead = true;

        await _db.SaveChangesAsync();
    }
}