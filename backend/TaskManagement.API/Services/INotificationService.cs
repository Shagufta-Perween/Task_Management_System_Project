using TaskManagement.API.DTOs.Notifications;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string message, NotificationType type, int? taskItemId);
        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task MarkAllAsReadAsync(string userId);
    }
}
