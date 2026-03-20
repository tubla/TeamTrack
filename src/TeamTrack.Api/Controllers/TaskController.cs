using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tasks")]
    [Authorize]
    public class TaskController(ITaskService service) : ControllerBase
    {
        private readonly ITaskService _service = service;

        [HttpPost]
        [HasPermission("task.create")]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpGet]
        [HasPermission("task.view")]
        public async Task<IActionResult> Get([FromQuery] QueryParams param, Guid projectId)
        {
            var result = await _service.GetAsync(param, projectId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(UpdateTaskStatusDto dto)
        {
            await _service.UpdateStatusAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Updated"));
        }
    }
}