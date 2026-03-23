using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Attributes;
using TeamTrack.Api.Common;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac.Constants;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/permissions")]
    [Authorize]
    public class PermissionController(IPermissionService service) : ControllerBase
    {
        private readonly IPermissionService _service = service;

        [HttpGet]
        [HasPermission(PermissionConstants.ViewPermission)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAllAsync();
            return Ok(ApiResponse<object>.SuccessResponse(result));
        }
    }
}