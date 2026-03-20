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
    [Route("api/v{version:apiVersion}/organizations")]
    [Authorize]
    public class OrganizationController(IOrganizationService service) : ControllerBase
    {
        private readonly IOrganizationService _service = service;

        /// <summary>
        /// Create a new organization
        /// </summary>
        [HttpPost]
        [HasPermission("org.create")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Create(CreateOrganizationDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Organization created"));
        }
    }
}