using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Organization;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class OrganizationService(
        ApplicationDbContext db, 
        IRequestContext context,
        IHttpContextAccessor httpContextAccessor,
        ILogger<OrganizationService> logger) : IOrganizationService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly ILogger<OrganizationService> _logger = logger;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<object> CreateAsync(CreateOrganizationDto dto)
        {
            var userId = _context.UserId;
            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
            
            _logger.LogInformation("Creating organization for {UserId} | CorrelationId: {CorrelationId}", userId, correlationId);

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

            _logger.LogInformation("Organization {Name} created for {UserId} | CorrelationId: {CorrelationId}", dto.Name, userId, correlationId);
            
            return new { org.Id, org.Name };
        }

        public async Task<List<OrganizationDto>> GetUserOrganizationsAsync()
        {
            var userId = _context.UserId;

            return await _db.OrganizationUsers
                .Where(ou => ou.UserId == userId)
                .Include(ou => ou.Organization)
                .Select(ou => new OrganizationDto
                {
                    Id = ou.OrganizationId,
                    Name = ou.Organization.Name
                })
                .ToListAsync();
        }

        public async Task<object> GetCurrentOrganizationAsync()
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var org = await _db.Organizations
                .Where(o => o.Id == orgId)
                .Select(o => new
                {
                    o.Id,
                    o.Name,
                    o.OwnerUserId,
                    MemberCount = _db.OrganizationUsers.Count(ou => ou.OrganizationId == o.Id)
                })
                .FirstOrDefaultAsync();

            if (org == null)
                throw new NotFoundException("Organization not found");

            return org;
        }

        public async Task<object> GetOrganizationMembersAsync()
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            return await _db.OrganizationUsers
                .Where(ou => ou.OrganizationId == orgId)
                .Include(ou => ou.User)
                .Select(ou => new
                {
                    ou.User.Id,
                    ou.User.FirstName,
                    ou.User.LastName,
                    ou.User.Email,
                    JoinedAt = ou.CreatedAt
                })
                .ToListAsync();
        }

        public async Task InviteUserAsync(InviteUserDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new NotFoundException("User not found");

            var alreadyMember = await _db.OrganizationUsers
                .AnyAsync(ou => ou.UserId == user.Id && ou.OrganizationId == orgId);

            if (alreadyMember)
                throw new BadRequestException("User is already a member of this organization");

            _db.OrganizationUsers.Add(new OrganizationUser
            {
                UserId = user.Id,
                OrganizationId = orgId
            });

            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserAsync(Guid userId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var org = await _db.Organizations.FindAsync(orgId);
            if (org == null)
                throw new NotFoundException("Organization not found");

            if (org.OwnerUserId == userId)
                throw new BadRequestException("Cannot remove the organization owner");

            var membership = await _db.OrganizationUsers
                .FirstOrDefaultAsync(ou => ou.UserId == userId && ou.OrganizationId == orgId);

            if (membership == null)
                throw new NotFoundException("User is not a member of this organization");

            _db.OrganizationUsers.Remove(membership);
            await _db.SaveChangesAsync();
        }

        public async Task<OrganizationDetailDto> GetOrganizationDetailAsync()
        {
            var orgId = _context.OrganizationId;

            if (orgId == null)
                throw new UnauthorizedAccessException("No organization context");

            var org = await _db.Organizations
                .Where(o => o.Id == orgId)
                .Select(o => new OrganizationDetailDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Description = o.Description,
                    CreatedAt = o.CreatedAt,
                    MemberCount = _db.OrganizationUsers.Count(ou => ou.OrganizationId == o.Id),
                    ProjectCount = _db.Projects.Count(p => p.OrganizationId == o.Id),
                    OwnerId = o.OwnerUserId,
                    OwnerName = o.OwnerUser.FirstName + " " + o.OwnerUser.LastName
                })
                .FirstOrDefaultAsync();

            if (org == null)
                throw new KeyNotFoundException("Organization not found");

            return org;
        }

        public async Task<OrganizationDetailDto> UpdateOrganizationAsync(UpdateOrganizationDto dto)
        {
            var orgId = _context.OrganizationId;

            if (orgId == null)
                throw new UnauthorizedAccessException("No organization context");

            var org = await _db.Organizations.FindAsync(orgId);

            if (org == null)
                throw new KeyNotFoundException("Organization not found");

            org.Name = dto.Name;
            org.Description = dto.Description;
            org.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();

            return await GetOrganizationDetailAsync();
        }
    }
}