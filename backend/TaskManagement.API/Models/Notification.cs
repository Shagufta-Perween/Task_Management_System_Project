using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public enum NotificationType
    {
        TaskAssigned,
        StatusUpdate
    }

    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public NotificationType Type { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;

        public int? TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }
    }
}
