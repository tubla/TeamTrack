using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Profile;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/profile")]
    [ApiVersion("1.0")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _service;

        public ProfileController(IProfileService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get current user's profile
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), 200)]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _service.GetProfileAsync();
            return Ok(ApiResponse<ProfileDto>.SuccessResponse(result));
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), 200)]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            var result = await _service.UpdateProfileAsync(dto);
            return Ok(ApiResponse<ProfileDto>.SuccessResponse(result, "Profile updated successfully"));
        }

        /// <summary>
        /// Change current user's password
        /// </summary>
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            await _service.ChangePasswordAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Password changed successfully"));
        }
    }
}
