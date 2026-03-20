using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize]
    public class NotificationController(INotificationService service) : ControllerBase
    {
        private readonly INotificationService _service = service;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] QueryParams param)
        {
            var result = await _service.GetMyAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _service.MarkAsReadAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Marked as read"));
        }
    }
}