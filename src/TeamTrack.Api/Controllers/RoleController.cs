using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("1.0")]
    [Authorize]
    public class RoleController(IRoleService service) : ControllerBase
    {
        private readonly IRoleService _service = service;

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            var result = await _service.CreateRoleAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> AssignPermissions(AssignPermissionsDto dto)
        {
            await _service.AssignPermissionsAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Permissions assigned"));
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole(AssignRoleToUserDto dto)
        {
            await _service.AssignRoleToUserAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Role assigned"));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] QueryParams param)
        {
            var result = await _service.GetRolesAsync(param);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}