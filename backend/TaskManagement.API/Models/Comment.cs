using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
    }
}
