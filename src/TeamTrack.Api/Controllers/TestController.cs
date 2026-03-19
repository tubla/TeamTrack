using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TeamTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("secure")]
        [Authorize]
        public IActionResult Get()
        {
            var user = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User?.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new 
            { 
                message = "API is working 🚀",
                userId = user,
                email = email,
                isAuthenticated = User?.Identity?.IsAuthenticated ?? false
            });
        }

        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new 
            { 
                message = "Public endpoint - no auth required",
                isAuthenticated = User?.Identity?.IsAuthenticated ?? false
            });
        }
    }
}