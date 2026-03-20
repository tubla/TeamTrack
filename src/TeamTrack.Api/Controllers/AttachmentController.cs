using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/attachments")]
    [Authorize]
    public class AttachmentController(IAttachmentService service) : ControllerBase
    {
        private readonly IAttachmentService _service = service;

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, Guid? taskId)
        {
            var result = await _service.UploadAsync(file, taskId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpGet("{taskId}")]
        public async Task<IActionResult> Get(Guid taskId)
        {
            var result = await _service.GetByTaskAsync(taskId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}
