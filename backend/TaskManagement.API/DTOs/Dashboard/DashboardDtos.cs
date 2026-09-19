namespace TaskManagement.API.DTOs.Dashboard
{
    /// <summary>
    /// Dashboard summary analytics DTO
    /// </summary>
    public class DashboardDto
    {
        /// <summary>
        /// Total number of tasks accessible to the current user
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Number of tasks currently in ToDo status
        /// </summary>
        public int TodoCount { get; set; }

        /// <summary>
        /// Number of tasks currently InProgress
        /// </summary>
        public int InProgressCount { get; set; }

        /// <summary>
        /// Number of completed (Done) tasks
        /// </summary>
        public int DoneCount { get; set; }

        /// <summary>
        /// Number of uncompleted tasks past their deadline
        /// </summary>
        public int OverdueTasks { get; set; }

        /// <summary>
        /// Number of high-priority tasks
        /// </summary>
        public int HighPriorityTasks { get; set; }

        /// <summary>
        /// List of 10 most recent task summaries
        /// </summary>
        public List<TaskSummaryDto> RecentTasks { get; set; } = new();
    }

    /// <summary>
    /// Lightweight summary representation of a task for dashboard lists
    /// </summary>
    public class TaskSummaryDto
    {
        /// <summary>
        /// Task unique ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Task title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Current task status (ToDo, InProgress, Done)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Task priority level (Low, Medium, High)
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Due date/time (UTC)
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Full name of assigned assignee
        /// </summary>
        public string AssignedToName { get; set; } = string.Empty;

        /// <summary>
        /// Name of the team associated with the task
        /// </summary>
        public string TeamName { get; set; } = string.Empty;
    }
}
