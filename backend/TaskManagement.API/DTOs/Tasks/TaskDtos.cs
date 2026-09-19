using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Tasks
{
    /// <summary>
    /// Parameters required to create a new task
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Task title
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the task requirements
        /// </summary>
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Task priority (Low, Medium, High). Default is Medium.
        /// </summary>
        [Required]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Optional due date/time for task completion (UTC)
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// ID of the team assigned to this task
        /// </summary>
        [Required]
        public int TeamId { get; set; }

        /// <summary>
        /// ID of the user assigned to complete this task
        /// </summary>
        [Required]
        public string AssignedToId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Parameters for updating an existing task (partial updates supported)
    /// </summary>
    public class UpdateTaskDto
    {
        /// <summary>
        /// Updated task title
        /// </summary>
        [StringLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// Updated detailed description
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Updated priority (Low, Medium, High)
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// Updated task status (ToDo, InProgress, Done)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Updated due date/time (UTC)
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Reassign to a different user ID
        /// </summary>
        public string? AssignedToId { get; set; }
    }

    /// <summary>
    /// Parameters for quick status update of a task
    /// </summary>
    public class UpdateTaskStatusDto
    {
        /// <summary>
        /// New task status (ToDo, InProgress, Done)
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Filter parameters for searching and listing tasks
    /// </summary>
    public class TaskFilterDto
    {
        /// <summary>
        /// Filter by task status (ToDo, InProgress, Done)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by priority (Low, Medium, High)
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// Filter tasks with deadline on or before this date
        /// </summary>
        public DateTime? DeadlineBefore { get; set; }

        /// <summary>
        /// Filter tasks with deadline on or after this date
        /// </summary>
        public DateTime? DeadlineAfter { get; set; }

        /// <summary>
        /// Filter tasks assigned to a specific team ID
        /// </summary>
        public int? TeamId { get; set; }
    }

    /// <summary>
    /// Detailed task item response representation
    /// </summary>
    public class TaskResponseDto
    {
        /// <summary>
        /// Unique task identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Task title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Current status (ToDo, InProgress, Done)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (Low, Medium, High)
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Due date/time (UTC)
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Creation timestamp (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last updated timestamp (UTC)
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// ID of assigned team
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Name of assigned team
        /// </summary>
        public string TeamName { get; set; } = string.Empty;

        /// <summary>
        /// ID of assigned user
        /// </summary>
        public string AssignedToId { get; set; } = string.Empty;

        /// <summary>
        /// Full name of assigned user
        /// </summary>
        public string AssignedToName { get; set; } = string.Empty;

        /// <summary>
        /// ID of user who created the task
        /// </summary>
        public string CreatedById { get; set; } = string.Empty;

        /// <summary>
        /// Full name of task creator
        /// </summary>
        public string CreatedByName { get; set; } = string.Empty;

        /// <summary>
        /// Number of comments posted on this task
        /// </summary>
        public int CommentCount { get; set; }
    }
}
