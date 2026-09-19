using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public TaskService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<List<TaskResponseDto>> GetTasksAsync(string userId, string userRole, TaskFilterDto? filter)
        {
            var query = _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .AsQueryable();

            // Role-based filtering
            if (userRole == "User")
            {
                query = query.Where(t => t.AssignedToId == userId);
            }
            else if (userRole == "Manager")
            {
                // Managers see tasks in their teams
                var managerTeamIds = await _context.TeamMembers
                    .Where(tm => tm.UserId == userId)
                    .Select(tm => tm.TeamId)
                    .ToListAsync();

                var createdTeamIds = await _context.Teams
                    .Where(t => t.CreatedById == userId)
                    .Select(t => t.Id)
                    .ToListAsync();

                var allTeamIds = managerTeamIds.Union(createdTeamIds).Distinct().ToList();
                query = query.Where(t => allTeamIds.Contains(t.TeamId));
            }
            // Admin sees all tasks

            // Apply filters
            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status) &&
                    Enum.TryParse<Models.TaskStatus>(filter.Status, true, out var status))
                {
                    query = query.Where(t => t.Status == status);
                }

                if (!string.IsNullOrEmpty(filter.Priority) &&
                    Enum.TryParse<TaskPriority>(filter.Priority, true, out var priority))
                {
                    query = query.Where(t => t.Priority == priority);
                }

                if (filter.DeadlineBefore.HasValue)
                {
                    query = query.Where(t => t.Deadline != null && t.Deadline <= filter.DeadlineBefore.Value);
                }

                if (filter.DeadlineAfter.HasValue)
                {
                    query = query.Where(t => t.Deadline != null && t.Deadline >= filter.DeadlineAfter.Value);
                }

                if (filter.TeamId.HasValue)
                {
                    query = query.Where(t => t.TeamId == filter.TeamId.Value);
                }
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id, string userId, string userRole)
        {
            var task = await _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;

            // Check access
            if (userRole == "User" && task.AssignedToId != userId)
                return null;

            return MapToDto(task);
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string createdById)
        {
            if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                throw new ArgumentException("Invalid priority value. Use: Low, Medium, or High.");

            var team = await _context.Teams.FindAsync(dto.TeamId);
            if (team == null)
                throw new ArgumentException("Team not found.");

            var assignedUser = await _context.Users.FindAsync(dto.AssignedToId);
            if (assignedUser == null)
                throw new ArgumentException("Assigned user not found.");

            var taskItem = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = priority,
                Status = Models.TaskStatus.ToDo,
                Deadline = dto.Deadline,
                TeamId = dto.TeamId,
                AssignedToId = dto.AssignedToId,
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            // Send notification to assigned user
            await _notificationService.CreateNotificationAsync(
                dto.AssignedToId,
                $"You have been assigned a new task: \"{dto.Title}\"",
                NotificationType.TaskAssigned,
                taskItem.Id
            );

            // Reload with includes
            var created = await _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .FirstAsync(t => t.Id == taskItem.Id);

            return MapToDto(created);
        }

        public async Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId, string userRole)
        {
            var task = await _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;

            if (!string.IsNullOrEmpty(dto.Priority))
            {
                if (!Enum.TryParse<TaskPriority>(dto.Priority, true, out var priority))
                    throw new ArgumentException("Invalid priority value.");
                task.Priority = priority;
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                if (!Enum.TryParse<Models.TaskStatus>(dto.Status, true, out var status))
                    throw new ArgumentException("Invalid status value.");

                var oldStatus = task.Status;
                task.Status = status;

                if (oldStatus != status)
                {
                    await _notificationService.CreateNotificationAsync(
                        task.AssignedToId,
                        $"Task \"{task.Title}\" status changed to {status}",
                        NotificationType.StatusUpdate,
                        task.Id
                    );
                }
            }

            if (dto.Deadline.HasValue) task.Deadline = dto.Deadline;

            if (!string.IsNullOrEmpty(dto.AssignedToId) && dto.AssignedToId != task.AssignedToId)
            {
                var newUser = await _context.Users.FindAsync(dto.AssignedToId);
                if (newUser == null) throw new ArgumentException("Assigned user not found.");

                task.AssignedToId = dto.AssignedToId;

                await _notificationService.CreateNotificationAsync(
                    dto.AssignedToId,
                    $"You have been assigned task: \"{task.Title}\"",
                    NotificationType.TaskAssigned,
                    task.Id
                );
            }

            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return MapToDto(task);
        }

        public async Task<TaskResponseDto> UpdateTaskStatusAsync(int id, string status, string userId, string userRole)
        {
            var task = await _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            // Users can only update their own tasks
            if (userRole == "User" && task.AssignedToId != userId)
                throw new UnauthorizedAccessException("You can only update your own tasks.");

            if (!Enum.TryParse<Models.TaskStatus>(status, true, out var newStatus))
                throw new ArgumentException("Invalid status. Use: ToDo, InProgress, or Done.");

            var oldStatus = task.Status;
            task.Status = newStatus;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify if status changed
            if (oldStatus != newStatus)
            {
                // Notify the task creator about status change
                if (task.CreatedById != userId)
                {
                    await _notificationService.CreateNotificationAsync(
                        task.CreatedById,
                        $"Task \"{task.Title}\" status changed from {oldStatus} to {newStatus}",
                        NotificationType.StatusUpdate,
                        task.Id
                    );
                }

                // Notify assigned user if different from who made the change
                if (task.AssignedToId != userId)
                {
                    await _notificationService.CreateNotificationAsync(
                        task.AssignedToId,
                        $"Task \"{task.Title}\" status changed to {newStatus}",
                        NotificationType.StatusUpdate,
                        task.Id
                    );
                }
            }

            return MapToDto(task);
        }

        public async Task DeleteTaskAsync(int id, string userId, string userRole)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }

        private static TaskResponseDto MapToDto(TaskItem task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                Deadline = task.Deadline,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                TeamId = task.TeamId,
                TeamName = task.Team?.Name ?? "",
                AssignedToId = task.AssignedToId,
                AssignedToName = task.AssignedTo?.FullName ?? "",
                CreatedById = task.CreatedById,
                CreatedByName = task.CreatedBy?.FullName ?? "",
                CommentCount = task.Comments?.Count ?? 0
            };
        }
    }
}
