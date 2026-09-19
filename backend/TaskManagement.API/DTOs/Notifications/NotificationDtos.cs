namespace TaskManagement.API.DTOs.Notifications
{
    /// <summary>
    /// User notification response DTO
    /// </summary>
    public class NotificationResponseDto
    {
        /// <summary>
        /// Unique notification identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Notification message content
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Notification category/type (TaskAssignment, StatusChange, Comment, DeadlineApproaching)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user has marked this notification as read
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Timestamp when the notification was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Associated task ID (if applicable)
        /// </summary>
        public int? TaskItemId { get; set; }
    }
}
