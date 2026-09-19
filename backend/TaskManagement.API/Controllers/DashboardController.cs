using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Dashboard;
using TaskManagement.API.Models;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Dashboard metrics and aggregated task analytics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard analytics overview and recent task activity
        /// </summary>
        /// <remarks>
        /// Computes role-scoped metric counts (Total, ToDo, InProgress, Done, Overdue, HighPriority) and 10 recent tasks.
        /// - **Admin**: Evaluates across all system tasks.
        /// - **Manager**: Evaluates tasks in managed/member teams.
        /// - **User**: Evaluates assigned tasks only.
        /// </remarks>
        /// <param name="status">Optional status filter (ToDo, InProgress, Done).</param>
        /// <param name="priority">Optional priority filter (Low, Medium, High).</param>
        /// <param name="deadline">Optional deadline cutoff date filter.</param>
        /// <returns>Dashboard metrics summary and recent task items.</returns>
        /// <response code="200">Dashboard metrics computed successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardDto>> GetDashboard([FromQuery] string? status, [FromQuery] string? priority, [FromQuery] DateTime? deadline)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;

            var query = _context.TaskItems
                .Include(t => t.Team)
                .Include(t => t.AssignedTo)
                .AsQueryable();

            // Role-based visibility
            if (userRole == "User")
            {
                query = query.Where(t => t.AssignedToId == userId);
            }
            else if (userRole == "Manager")
            {
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

            // Optional query filters
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<Models.TaskStatus>(status, true, out var sVal))
            {
                query = query.Where(t => t.Status == sVal);
            }
            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<TaskPriority>(priority, true, out var pVal))
            {
                query = query.Where(t => t.Priority == pVal);
            }
            if (deadline.HasValue)
            {
                query = query.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date <= deadline.Value.Date);
            }

            var allFilteredTasks = await query.ToListAsync();
            var now = DateTime.UtcNow;

            var dto = new DashboardDto
            {
                TotalTasks = allFilteredTasks.Count,
                TodoCount = allFilteredTasks.Count(t => t.Status == Models.TaskStatus.ToDo),
                InProgressCount = allFilteredTasks.Count(t => t.Status == Models.TaskStatus.InProgress),
                DoneCount = allFilteredTasks.Count(t => t.Status == Models.TaskStatus.Done),
                OverdueTasks = allFilteredTasks.Count(t => t.Deadline.HasValue && t.Deadline.Value < now && t.Status != Models.TaskStatus.Done),
                HighPriorityTasks = allFilteredTasks.Count(t => t.Priority == TaskPriority.High),
                RecentTasks = allFilteredTasks
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(10)
                    .Select(t => new TaskSummaryDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                        Priority = t.Priority.ToString(),
                        Deadline = t.Deadline,
                        AssignedToName = t.AssignedTo?.FullName ?? "",
                        TeamName = t.Team?.Name ?? ""
                    })
                    .ToList()
            };

            return Ok(dto);
        }
    }
}
