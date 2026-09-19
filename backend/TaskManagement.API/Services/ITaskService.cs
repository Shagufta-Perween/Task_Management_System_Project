using TaskManagement.API.DTOs.Tasks;

namespace TaskManagement.API.Services
{
    public interface ITaskService
    {
        Task<List<TaskResponseDto>> GetTasksAsync(string userId, string userRole, TaskFilterDto? filter);
        Task<TaskResponseDto?> GetTaskByIdAsync(int id, string userId, string userRole);
        Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string createdById);
        Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId, string userRole);
        Task<TaskResponseDto> UpdateTaskStatusAsync(int id, string status, string userId, string userRole);
        Task DeleteTaskAsync(int id, string userId, string userRole);
    }
}
