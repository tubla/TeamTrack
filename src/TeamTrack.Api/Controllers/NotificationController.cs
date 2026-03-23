using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        private readonly INotificationService _service = service;

        /// <summary>
        /// Get all notifications for current user
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewNotification)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Get([FromQuery] QueryParams param)
        {
            var result = await _service.GetMyAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread-count")]
        [HasPermission(PermissionConstants.ViewNotification)]
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _service.GetUnreadCountAsync();
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }

        /// <summary>
        /// Mark single notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        [HasPermission(PermissionConstants.UpdateNotification)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _service.MarkAsReadAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Marked as read"));
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("read-all")]
        [HasPermission(PermissionConstants.UpdateNotification)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _service.MarkAllAsReadAsync();
            return Ok(ApiResponse<string>.SuccessResponse("All notifications marked as read"));
        }
    }
}