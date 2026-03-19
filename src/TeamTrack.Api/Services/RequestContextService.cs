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
                var orgId = _httpContextAccessor.HttpContext?.Request.Headers["X-Organization-Id"].FirstOrDefault();

                return Guid.TryParse(orgId, out var id) ? id : null;
            }
        }
    }
}