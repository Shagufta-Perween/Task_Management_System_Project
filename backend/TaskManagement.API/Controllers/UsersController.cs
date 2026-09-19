using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Users;
using TaskManagement.API.Models;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// User account management and role administration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public UsersController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Get all registered users in system (Restricted to Admin role)
        /// </summary>
        /// <returns>List of user summary objects with assigned roles.</returns>
        /// <response code="200">Returns complete list of system users.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin role.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "User",
                    CreatedAt = user.CreatedAt
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all users for task/team assignment dropdowns (Admin &amp; Manager roles)
        /// </summary>
        /// <returns>List of user summaries for selection UI.</returns>
        /// <response code="200">List of users retrieved.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "User",
                    CreatedAt = user.CreatedAt
                });
            }

            return Ok(result);
        }

        /// <summary>
        /// Get user details by user ID (Restricted to Admin role)
        /// </summary>
        /// <param name="id">User unique identifier (GUID format).</param>
        /// <returns>User profile detail.</returns>
        /// <response code="200">User record found.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? "User",
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Update a user's role (Restricted to Admin role only)
        /// </summary>
        /// <remarks>
        /// Valid roles: Admin, Manager, User.
        /// </remarks>
        /// <param name="id">Target user ID.</param>
        /// <param name="dto">Role update payload.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">User role updated successfully.</response>
        /// <response code="400">Invalid role specified.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            var validRoles = new[] { "Admin", "Manager", "User" };
            if (!validRoles.Contains(dto.Role))
                return BadRequest(new { message = "Invalid role. Valid roles: Admin, Manager, User." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            return Ok(new { message = $"User role updated to {dto.Role}" });
        }

        /// <summary>
        /// Delete user account (Restricted to Admin role only)
        /// </summary>
        /// <remarks>
        /// Cleans up foreign key dependencies (team memberships, comments, notifications, task assignments) before deleting user profile.
        /// Users cannot delete their own active account.
        /// </remarks>
        /// <param name="id">Target user ID.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">User deleted successfully.</response>
        /// <response code="400">Attempted self-deletion or deletion error.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin role.</response>
        /// <response code="404">User not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == currentUserId)
                return BadRequest(new { message = "You cannot delete your own account." });

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            // 1. Remove team memberships
            var teamMemberships = await _context.TeamMembers.Where(tm => tm.UserId == id).ToListAsync();
            _context.TeamMembers.RemoveRange(teamMemberships);

            // 2. Remove user notifications
            var notifications = await _context.Notifications.Where(n => n.UserId == id).ToListAsync();
            _context.Notifications.RemoveRange(notifications);

            // 3. Remove user comments
            var comments = await _context.Comments.Where(c => c.UserId == id).ToListAsync();
            _context.Comments.RemoveRange(comments);

            // 4. Reassign tasks assigned to this user -> assign to active Admin (currentUserId)
            var assignedTasks = await _context.TaskItems.Where(t => t.AssignedToId == id).ToListAsync();
            foreach (var task in assignedTasks)
            {
                task.AssignedToId = currentUserId!;
            }

            // 5. Reassign tasks created by this user -> set CreatedById to active Admin (currentUserId)
            var createdTasks = await _context.TaskItems.Where(t => t.CreatedById == id).ToListAsync();
            foreach (var task in createdTasks)
            {
                task.CreatedById = currentUserId!;
            }

            // 6. Reassign teams created by this user -> set CreatedById to active Admin (currentUserId)
            var createdTeams = await _context.Teams.Where(t => t.CreatedById == id).ToListAsync();
            foreach (var team in createdTeams)
            {
                team.CreatedById = currentUserId!;
            }

            await _context.SaveChangesAsync();

            // 7. Delete ASP.NET Identity User
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Failed to delete user: {errorMsg}" });
            }

            return Ok(new { message = "User deleted successfully" });
        }
    }
}
