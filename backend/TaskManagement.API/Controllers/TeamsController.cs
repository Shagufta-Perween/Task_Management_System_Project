using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Teams;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Team creation, membership management, and team assignment
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        /// <summary>
        /// Get all teams (scoped by user role)
        /// </summary>
        /// <returns>List of team objects with membership info.</returns>
        /// <response code="200">List of teams returned.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<TeamResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<TeamResponseDto>>> GetTeams()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var teams = await _teamService.GetTeamsAsync(userId, userRole);
            return Ok(teams);
        }

        /// <summary>
        /// Get team details by ID
        /// </summary>
        /// <param name="id">Team identifier.</param>
        /// <returns>Team details with complete member list.</returns>
        /// <response code="200">Team object found.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeamResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamResponseDto>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null) return NotFound();
            return Ok(team);
        }

        /// <summary>
        /// Create a new team (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="dto">Team parameters (Name, Description).</param>
        /// <returns>Newly created team object with 201 Created status.</returns>
        /// <response code="201">Team created successfully.</response>
        /// <response code="400">Invalid payload or duplicate team name.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(TeamResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TeamResponseDto>> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var team = await _teamService.CreateTeamAsync(dto, userId);
            return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
        }

        /// <summary>
        /// Update team details (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="id">Team identifier.</param>
        /// <param name="dto">Fields to update.</param>
        /// <returns>Updated team object.</returns>
        /// <response code="200">Team updated successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        /// <response code="404">Team not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(TeamResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamResponseDto>> UpdateTeam(int id, [FromBody] UpdateTeamDto dto)
        {
            var team = await _teamService.UpdateTeamAsync(id, dto);
            return Ok(team);
        }

        /// <summary>
        /// Delete a team (Restricted to Admin role only)
        /// </summary>
        /// <param name="id">Team identifier.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">Team deleted successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin role.</response>
        /// <response code="404">Team not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            await _teamService.DeleteTeamAsync(id);
            return Ok(new { message = "Team deleted successfully" });
        }

        /// <summary>
        /// Add a user to team members (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="id">Team identifier.</param>
        /// <param name="dto">User ID to add.</param>
        /// <returns>Updated team record with members list.</returns>
        /// <response code="200">Member added successfully.</response>
        /// <response code="400">User is already a team member or user ID invalid.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        /// <response code="404">Team or user not found.</response>
        [HttpPost("{id}/members")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(TeamResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamResponseDto>> AddMember(int id, [FromBody] AddTeamMemberDto dto)
        {
            var team = await _teamService.AddMemberAsync(id, dto.UserId);
            return Ok(team);
        }

        /// <summary>
        /// Remove a member from a team (Restricted to Admin &amp; Manager roles)
        /// </summary>
        /// <param name="id">Team identifier.</param>
        /// <param name="userId">User ID to remove.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">Member removed successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="403">Forbidden. Requires Admin or Manager role.</response>
        /// <response code="404">Team or member assignment not found.</response>
        [HttpDelete("{id}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(int id, string userId)
        {
            await _teamService.RemoveMemberAsync(id, userId);
            return Ok(new { message = "Member removed successfully" });
        }
    }
}
