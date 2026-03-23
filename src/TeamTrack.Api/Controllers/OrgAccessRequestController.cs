using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.DTOs.OrgAccess;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/org-access")]
    [Authorize]
    public class OrgAccessRequestController(IOrgAccessService service) : ControllerBase
    {
        private readonly IOrgAccessService _service = service;

        /// <summary>
        /// Called by a NEW USER → Request access to an organization.
        /// No organizationId or adminUserId involved.
        /// </summary>
        [HttpPost("request")]
        public async Task<IActionResult> RequestAccess([FromBody] OrgAccessRequestDto dto)
        {
            var result = await _service.CreateRequestAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Called by an ADMIN → Approve a user's request and assign them to an organization.
        /// </summary>
        [HttpPost("approve")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Approve([FromBody] ApproveOrgAccessRequestDto dto)
        {
            var result = await _service.ApproveRequestAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Called by an ADMIN → Reject a user's request.
        /// </summary>
        [HttpPost("reject/{requestId:guid}")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Reject(Guid requestId)
        {
            var result = await _service.RejectRequestAsync(requestId);
            return Ok(result);
        }

        /// <summary>
        /// (Optional) Admin can view all pending access requests.
        /// Useful for building the Admin UI.
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var result = await _service.GetPendingRequestsAsync();
            return Ok(result);
        }
    }
}