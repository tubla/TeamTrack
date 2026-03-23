using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Dashboard;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/dashboard")]
    [Authorize]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        private readonly IDashboardService _service = service;

        /// <summary>
        /// Get dashboard data for current organization
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<DashboardDto>), 200)]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _service.GetDashboardAsync();
            return Ok(ApiResponse<DashboardDto>.SuccessResponse(result));
        }
    }
}