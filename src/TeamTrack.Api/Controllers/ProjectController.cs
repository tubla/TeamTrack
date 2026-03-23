using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Project;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/projects")]
    [Authorize]
    public class ProjectController(IProjectService service) : ControllerBase
    {
        private readonly IProjectService _service = service;

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.CreateProject)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Project created"));
        }

        /// <summary>
        /// Get all projects in current organization
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewProject)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Get([FromQuery] QueryParams param)
        {
            var result = await _service.GetAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Get project by ID with details
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission(PermissionConstants.ViewProject)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Update project
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission(PermissionConstants.UpdateProject)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Project updated"));
        }

        /// <summary>
        /// Delete project
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission(PermissionConstants.DeleteProject)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Project deleted"));
        }
    }
}