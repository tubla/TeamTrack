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
    [Route("api/v{version:apiVersion}/attachments")]
    [Authorize]
    public class AttachmentController(IAttachmentService service) : ControllerBase
    {
        private readonly IAttachmentService _service = service;

        /// <summary>
        /// Upload attachment (optionally linked to a task)
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.UploadAttachment)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] Guid? taskId)
        {
            var result = await _service.UploadAsync(file, taskId);
            return Ok(ApiResponse<object>.SuccessResponse(result, "File uploaded"));
        }

        /// <summary>
        /// Get attachments for a task
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewAttachment)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetByTask([FromQuery] Guid taskId)
        {
            var result = await _service.GetByTaskAsync(taskId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Delete attachment
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission(PermissionConstants.UploadAttachment)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Attachment deleted"));
        }
    }
}
