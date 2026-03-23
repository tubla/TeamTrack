using TeamTrack.Api.Common;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, Guid? organizationId, string title, string message, NotificationType type, Guid? referenceId);
        Task CreateAsync(string title, string message, NotificationType type, Guid? referenceId);

        Task<object> GetMyAsync(QueryParams param);
        Task<int> GetUnreadCountAsync();
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync();
    }
}
