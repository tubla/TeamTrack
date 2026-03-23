using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Comment;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/comments")]
    [Authorize]
    public class CommentController(ICommentService service) : ControllerBase
    {
        private readonly ICommentService _service = service;

        /// <summary>
        /// Add comment to a task
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.CreateComment)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateCommentDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Comment added"));
        }

        /// <summary>
        /// Get comments for a task
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewComment)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetByTask([FromQuery] Guid taskId)
        {
            var result = await _service.GetByTaskAsync(taskId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}