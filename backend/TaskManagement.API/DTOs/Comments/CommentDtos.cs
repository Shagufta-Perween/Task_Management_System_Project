using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Comments
{
    /// <summary>
    /// Payload for posting a comment on a task
    /// </summary>
    public class CreateCommentDto
    {
        /// <summary>
        /// Text content of the comment (up to 1000 characters)
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// Comment details response DTO
    /// </summary>
    public class CommentResponseDto
    {
        /// <summary>
        /// Unique comment identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Comment text content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the comment was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Author's unique user ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Author's full name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// ID of the parent task
        /// </summary>
        public int TaskItemId { get; set; }
    }
}
