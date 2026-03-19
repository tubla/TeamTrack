using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class OrganizationService(ApplicationDbContext db, IRequestContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger) : IOrganizationService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<object> CreateAsync(CreateOrganizationDto dto)
        {

            var userId = _context.UserId;

            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
            _logger.LogInformation("Creating organization for {UserId}| CorrelationId: {CorrelationId}", userId, correlationId);
                        

            var org = new Organization
            {
                Name = dto.Name,
                OwnerUserId = userId
            };

            _db.Organizations.Add(org);

            _db.OrganizationUsers.Add(new OrganizationUser
            {
                UserId = userId,
                OrganizationId = org.Id
            });

            await _db.SaveChangesAsync();

            _logger.LogInformation("Organization {name} created for {UserId}| CorrelationId: {CorrelationId}",dto.Name, userId, correlationId);
            return new
            {
                org.Id,
                org.Name
            };
        }
    }
}