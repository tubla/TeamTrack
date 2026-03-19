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

        public async Task<string> GenerateToken(User user)
        {
            // Add permissions into JWT
            var permissions = await GetUserPermissions(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(permissions.Select(p =>
                new Claim("permission", p)));

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

        private async Task<List<string>> GetUserPermissions(User user)
        {
            var cacheKey = $"permissions:{user.Id}";

            var permissions = await _cache.GetAsync<List<string>>(cacheKey);

            if (permissions == null)
            {
                permissions = await _db.UserRoles
                    .Where(ur => ur.UserId == user.Id)
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
