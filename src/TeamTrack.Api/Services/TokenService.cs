using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamTrack.Api.Data;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class TokenService(ApplicationDbContext db, ICacheService cache, IConfiguration config) : ITokenService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IConfiguration _config = config;
        private readonly ICacheService _cache = cache;

        public async Task<string> GenerateToken(User user, Guid? organizationId = null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new("firstName", user.FirstName),
                new("lastName", user.LastName)
            };

            // Only add organizationId if provided (after org selection)
            if (organizationId.HasValue && organizationId.Value != Guid.Empty)
            {
                claims.Add(new Claim("organizationId", organizationId.Value.ToString()));
                
                // Add org-scoped permissions
                var permissions = await GetUserPermissions(user.Id, organizationId.Value);
                claims.AddRange(permissions.Select(p => new Claim("permission", p)));

                // Add role claims (so Authorize(Roles="Admin,SuperAdmin") works)
                var roles = await _db.UserRoles
                    .Where(ur => ur.UserId == user.Id && ur.OrganizationId == organizationId.Value)
                    .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .Distinct()
                    .ToListAsync();

                // Add both ClaimTypes.Role and "role" for compatibility
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                claims.AddRange(roles.Select(r => new Claim("role", r)));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<List<string>> GetUserPermissions(Guid userId, Guid organizationId)
        {
            var cacheKey = $"permissions:{userId}:{organizationId}";

            var permissions = await _cache.GetAsync<List<string>>(cacheKey);

            if (permissions == null)
            {
                permissions = await _db.UserRoles
                    .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
                    .Join(_db.RolePermissions,
                        ur => ur.RoleId,
                        rp => rp.RoleId,
                        (ur, rp) => rp)
                    .Join(_db.Permissions,
                        rp => rp.PermissionId,
                        p => p.Id,
                        (rp, p) => p.Name)
                    .Distinct()
                    .ToListAsync();

                await _cache.SetAsync(cacheKey, permissions, TimeSpan.FromMinutes(30));
            }

            return permissions;
        }
    }    
}
