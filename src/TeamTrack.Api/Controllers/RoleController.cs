using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Permission;
using TeamTrack.Api.DTOs.Role;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/roles")]
    [ApiVersion("1.0")]
    [Authorize]
    public class RoleController(IRoleService service) : ControllerBase
    {
        private readonly IRoleService _service = service;

        /// <summary>
        /// Create a new role
        /// </summary>
        [HttpPost]
        [HasPermission(PermissionConstants.CreateRole)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            var result = await _service.CreateRoleAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Role created"));
        }

        /// <summary>
        /// Get all roles in current organization
        /// </summary>
        [HttpGet]
        [HasPermission(PermissionConstants.ViewRole)]
        [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), 200)]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetRolesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get role details with permissions
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission(PermissionConstants.ViewRole)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetRoleWithPermissionsAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Assign permissions to a role
        /// </summary>
        [HttpPost("{id}/permissions")]
        [HasPermission(PermissionConstants.AssignRole)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AssignPermissions(Guid id, AssignPermissionsDto dto)
        {
            dto.RoleId = id;
            await _service.AssignPermissionsAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Permissions assigned"));
        }

        /// <summary>
        /// Assign role to a user
        /// </summary>
        [HttpPost("assign")]
        [HasPermission(PermissionConstants.AssignRole)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> AssignRole(AssignRoleToUserDto dto)
        {
            await _service.AssignRoleToUserAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Role assigned"));
        }

        /// <summary>
        /// Remove role from a user
        /// </summary>
        [HttpDelete("users/{userId}/roles/{roleId}")]
        [HasPermission(PermissionConstants.AssignRole)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId)
        {
            await _service.RemoveRoleFromUserAsync(userId, roleId);
            return Ok(ApiResponse<string>.SuccessResponse("Role removed"));
        }

        /// <summary>
        /// Get all permissions grouped by category
        /// </summary>
        [HttpGet("permissions")]
        [HasPermission(PermissionConstants.ViewRole)]
        [ProducesResponseType(typeof(ApiResponse<List<PermissionGroupDto>>), 200)]
        public IActionResult GetAllPermissions([FromServices] IPermissionService permissionService)
        {
            var result = permissionService.GetAllGroupedPermissions();
            return Ok(ApiResponse<List<PermissionGroupDto>>.SuccessResponse(result));
        }
    }
}