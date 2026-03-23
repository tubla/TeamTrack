using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Task;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tasks")]
    [Authorize]
    public class TaskController(ITaskService service) : ControllerBase
    {
        private readonly ITaskService _service = service;

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.CreateTask)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Task created"));
        }

        /// <summary>
        /// Get tasks by project
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewTask)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Get([FromQuery] QueryParams param, [FromQuery] Guid projectId)
        {
            var result = await _service.GetAsync(param, projectId);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Get task by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission(PermissionConstants.ViewTask)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPut("{id}/status")]
        [HasPermission(PermissionConstants.UpdateTask)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusDto dto)
        {
            dto.TaskId = id;
            await _service.UpdateStatusAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Status updated"));
        }

        /// <summary>
        /// Update task details
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission(PermissionConstants.UpdateTask)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Update(Guid id, UpdateTaskDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Task updated"));
        }

        /// <summary>
        /// Assign task to user
        /// </summary>
        [HttpPut("{id}/assign")]
        [HasPermission(PermissionConstants.UpdateTask)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AssignTask(Guid id, AssignTaskDto dto)
        {
            await _service.AssignTaskAsync(id, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Task assigned"));
        }

        /// <summary>
        /// Delete task
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission(PermissionConstants.DeleteTask)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Task deleted"));
        }

        /// <summary>
        /// Get my assigned tasks across all projects
        /// </summary>
        [HttpGet("my")]
        [HasPermission(PermissionConstants.ViewTask)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetMyTasks([FromQuery] QueryParams param)
        {
            var result = await _service.GetMyTasksAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}