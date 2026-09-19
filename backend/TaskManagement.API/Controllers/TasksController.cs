using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Tasks;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Task CRUD lifecycle management and task status tracking
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Get tasks list (supports status/priority/team filtering and role scoping)
        /// </summary>
        /// <remarks>
        /// - **Admin**: Access to all tasks across all teams.
        /// - **Manager**: Access to tasks created by or assigned to teams they manage.
        /// - **User**: Access to tasks directly assigned to them.
        /// </remarks>
        /// <param name="filter">Filtering parameters (status, priority, deadline, teamId).</param>
        /// <returns>Array of task response objects.</returns>
        /// <response code="200">Filtered list of tasks.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TaskResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TaskResponseDto>>> GetTasks([FromQuery] TaskFilterDto filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var tasks = await _taskService.GetTasksAsync(userId, userRole, filter);
            return Ok(tasks);
        }

        /// <summary>
        /// Get detailed task item by unique ID
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Full task details.</returns>
        /// <response code="200">Task object found.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Task not found or user lacks permission to view it.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var task = await _taskService.GetTaskByIdAsync(id, userId, userRole);
            if (task == null) return NotFound();
            return Ok(task);
        }

        /// <summary>
        /// Create a new task (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <remarks>
        /// Assigns task to specified team member and sends an automated assignment notification.
        /// </remarks>
        /// <param name="dto">Task creation parameters.</param>
        /// <returns>Newly created task record with 201 Created location header.</returns>
        /// <response code="201">Task created successfully.</response>
        /// <response code="400">Validation error or invalid team/assignee ID.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Required Admin or Manager role.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var task = await _taskService.CreateTaskAsync(dto, userId);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Update an existing task (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <param name="dto">Fields to update.</param>
        /// <returns>Updated task object.</returns>
        /// <response code="200">Task updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        /// <response code="404">Task not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var task = await _taskService.UpdateTaskAsync(id, dto, userId, userRole);
            return Ok(task);
        }

        /// <summary>
        /// Update task status (Accessible by Assignee, Manager, or Admin)
        /// </summary>
        /// <remarks>
        /// Allows assigned team members to change status (e.g. ToDo -> InProgress -> Done).
        /// Triggers notification to task creator on status update.
        /// </remarks>
        /// <param name="id">Task identifier.</param>
        /// <param name="dto">New status payload (ToDo, InProgress, Done).</param>
        /// <returns>Updated task object.</returns>
        /// <response code="200">Task status updated successfully.</response>
        /// <response code="400">Invalid status value.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Task not found.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskResponseDto>> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var task = await _taskService.UpdateTaskStatusAsync(id, dto.Status, userId, userRole);
            return Ok(task);
        }

        /// <summary>
        /// Delete a task (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="id">Task identifier.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">Task deleted successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        /// <response code="404">Task not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            await _taskService.DeleteTaskAsync(id, userId, userRole);
            return Ok(new { message = "Task deleted successfully" });
        }
    }
}
