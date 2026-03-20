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
    [Route("api/v{version:apiVersion}/projects")]
    [Authorize]
    public class ProjectController(IProjectService service) : ControllerBase
    {
        private readonly IProjectService _service = service;

        [HttpPost]
        [HasPermission("project.create")]
        public async Task<IActionResult> Create(CreateProjectDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpGet]
        [HasPermission("project.view")]
        public async Task<IActionResult> Get([FromQuery] QueryParams param)
        {
            var result = await _service.GetAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}