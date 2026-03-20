using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/comments")]
    [Authorize]
    public class CommentController(ICommentService service) : ControllerBase
    {
        private readonly ICommentService _service = service;

        [HttpPost]
        public async Task<IActionResult> Create(CreateCommentDto dto)
        {
            var result = await _service.CreateAsync(dto);
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