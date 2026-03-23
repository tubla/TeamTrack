using System.Security.Claims;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class RequestContextService : IRequestContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId =>
            Guid.Parse(_httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

        public string Email =>
            _httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        public Guid? OrganizationId
        {
            get
            {
                // First check the header
                var orgIdHeader = _httpContextAccessor.HttpContext?
                    .Request.Headers["X-Organization-Id"].FirstOrDefault();

                if (Guid.TryParse(orgIdHeader, out var headerId))
                    return headerId;

                // Fallback to JWT claim
                var orgClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("organizationId")?.Value;

                return Guid.TryParse(orgClaim, out var claimId) ? claimId : null;
            }
        }
    }
}