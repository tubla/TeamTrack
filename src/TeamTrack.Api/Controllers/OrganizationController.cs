using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Organization;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations")]
    [Authorize]
    public class OrganizationController(IOrganizationService service) : ControllerBase
    {
        private readonly IOrganizationService _service = service;

        /// <summary>
        /// Create a new organization
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateOrganizationDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Organization created"));
        }

        /// <summary>
        /// Get all organizations for current user (no org context required)
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(ApiResponse<List<OrganizationDto>>), 200)]
        public async Task<IActionResult> GetMyOrganizations()
        {
            var result = await _service.GetUserOrganizationsAsync();
            return Ok(ApiResponse<List<OrganizationDto>>.SuccessResponse(result));
        }        

        /// <summary>
        /// Get all members of current organization
        /// </summary>
        [HttpGet("members")]
        [HasPermission(PermissionConstants.ViewOrganization)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> GetMembers()
        {
            var result = await _service.GetOrganizationMembersAsync();
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }

        /// <summary>
        /// Invite a user to current organization
        /// </summary>
        [HttpPost("invite")]
        [HasPermission(PermissionConstants.CreateOrganization)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> InviteUser(InviteUserDto dto)
        {
            await _service.InviteUserAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("User invited successfully"));
        }

        /// <summary>
        /// Remove a user from current organization
        /// </summary>
        [HttpDelete("members/{userId}")]
        [HasPermission(PermissionConstants.CreateOrganization)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> RemoveUser(Guid userId)
        {
            await _service.RemoveUserAsync(userId);
            return Ok(ApiResponse<string>.SuccessResponse("User removed successfully"));
        }

        /// <summary>
        /// Get current organization details
        /// </summary>
        [HttpGet("details")]
        [HasPermission(PermissionConstants.ViewOrganization)]
        [ProducesResponseType(typeof(ApiResponse<OrganizationDetailDto>), 200)]
        public async Task<IActionResult> GetOrganizationDetail()
        {
            var result = await _service.GetOrganizationDetailAsync();
            return Ok(ApiResponse<OrganizationDetailDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Update current organization
        /// </summary>
        [HttpPut]
        [HasPermission(PermissionConstants.UpdateOrganization)]
        [ProducesResponseType(typeof(ApiResponse<OrganizationDetailDto>), 200)]
        public async Task<IActionResult> UpdateOrganization(UpdateOrganizationDto dto)
        {
            var result = await _service.UpdateOrganizationAsync(dto);
            return Ok(ApiResponse<OrganizationDetailDto>.SuccessResponse(result, "Organization updated"));
        }
    }
}