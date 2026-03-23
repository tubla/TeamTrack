using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Auth;
using TeamTrack.Api.DTOs.Organization;
using TeamTrack.Api.DTOs.Token;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [EnableRateLimiting("auth")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController(IAuthService service, IRequestContext requestContext) : ControllerBase
    {
        private readonly IAuthService _service = service;
        private readonly IRequestContext _requestContext = requestContext;

        /// <summary>
        /// Register new user
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<RegisterResponseDto>), 200)]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _service.RegisterAsync(dto);
            return Ok(ApiResponse<RegisterResponseDto>.SuccessResponse(result, "User registered"));
        }

        /// <summary>
        /// Login user - returns token and list of organizations
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
        }

        /// <summary>
        /// Switch to a different organization - returns new token with org context
        /// </summary>
        [Authorize]
        [HttpPost("switch-organization")]
        [ProducesResponseType(typeof(ApiResponse<SwitchOrganizationResponseDto>), 200)]
        public async Task<IActionResult> SwitchOrganization(SwitchOrganizationDto dto)
        {
            var userId = _requestContext.UserId;
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _service.SwitchOrganizationAsync(userId, dto);
            return Ok(ApiResponse<SwitchOrganizationResponseDto>.SuccessResponse(result, "Organization switched"));
        }

        /// <summary>
        /// Refresh token - extends session
        /// </summary>
        [Authorize]
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponseDto>), 200)]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = _requestContext.UserId;
            var orgId = _requestContext.OrganizationId;

            var result = await _service.RefreshTokenAsync(userId, orgId);
            return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(result, "Token refreshed"));
        }
    }
}