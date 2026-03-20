using TeamTrack.Api.Common;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(Guid userId, string title, string message, NotificationType type, Guid? referenceId);
        Task<object> GetMyAsync(QueryParams param);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
