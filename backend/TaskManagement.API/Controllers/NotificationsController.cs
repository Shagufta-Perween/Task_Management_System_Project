using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs.Notifications;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// User notifications and alert management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for the authenticated user
        /// </summary>
        /// <remarks>
        /// Returns notifications sorted with newest first.
        /// </remarks>
        /// <returns>List of notification items.</returns>
        /// <response code="200">Notifications retrieved successfully.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<NotificationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<NotificationResponseDto>>> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Get count of unread notifications for current user
        /// </summary>
        /// <returns>Object containing unread notification count.</returns>
        /// <response code="200">Returns integer count of unread notifications.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        /// <summary>
        /// Mark a single notification as read
        /// </summary>
        /// <param name="id">Notification unique ID.</param>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">Notification marked as read.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        /// <response code="404">Notification not found or does not belong to user.</response>
        [HttpPatch("{id}/read")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { message = "Marked as read" });
        }

        /// <summary>
        /// Mark all notifications as read for current user
        /// </summary>
        /// <returns>Confirmation message.</returns>
        /// <response code="200">All notifications updated.</response>
        /// <response code="401">Unauthorized access token missing or invalid.</response>
        [HttpPatch("read-all")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "All notifications marked as read" });
        }
    }
}
