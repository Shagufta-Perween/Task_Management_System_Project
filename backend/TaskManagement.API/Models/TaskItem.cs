using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }

    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public string AssignedToId { get; set; } = string.Empty;
        public AppUser AssignedTo { get; set; } = null!;

        public string CreatedById { get; set; } = string.Empty;
        public AppUser CreatedBy { get; set; } = null!;

        // Navigation properties
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
