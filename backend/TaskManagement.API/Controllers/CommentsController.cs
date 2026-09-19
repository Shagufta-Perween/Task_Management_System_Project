using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Comments;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Task discussion and comment management
    /// </summary>
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Retrieve all comments posted on a specific task
        /// </summary>
        /// <remarks>
        /// Requires authenticated user access. Returns list ordered by creation timestamp.
        /// </remarks>
        /// <param name="taskId">The target task identifier.</param>
        /// <returns>List of comments belonging to the specified task.</returns>
        /// <response code="200">Returns list of task comments.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Task not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<CommentResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CommentResponseDto>>> GetComments(int taskId)
        {
            var comments = await _commentService.GetCommentsAsync(taskId);
            return Ok(comments);
        }

        /// <summary>
        /// Add a new comment to a task
        /// </summary>
        /// <remarks>
        /// Authenticated user posts a comment. Automatically triggers notification to task assignee/creator.
        /// </remarks>
        /// <param name="taskId">The target task identifier.</param>
        /// <param name="dto">Comment body payload.</param>
        /// <returns>The newly created comment object.</returns>
        /// <response code="200">Comment successfully posted.</response>
        /// <response code="400">Invalid comment body.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Task not found.</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommentResponseDto>> AddComment(int taskId, [FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var comment = await _commentService.AddCommentAsync(taskId, dto, userId);
            return Ok(comment);
        }
    }
}
