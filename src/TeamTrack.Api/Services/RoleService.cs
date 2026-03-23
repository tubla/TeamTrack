using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Permission;
using TeamTrack.Api.DTOs.Role;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models.Rbac;

namespace TeamTrack.Api.Services
{
    public class RoleService(ApplicationDbContext db, ICacheService cache, IRequestContext context) : IRoleService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;
        private readonly ICacheService _cache = cache;

        public async Task<object> CreateRoleAsync(CreateRoleDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            if (await _db.Roles.AnyAsync(r => r.Name == dto.Name && r.OrganizationId == orgId))
                throw new BadRequestException("Role already exists");

            var role = new Role
            {
                Name = dto.Name,
                OrganizationId = orgId
            };

            _db.Roles.Add(role);
            await _db.SaveChangesAsync();

            return new { role.Id, role.Name };
        }

        public async Task AssignPermissionsAsync(AssignPermissionsDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId && r.OrganizationId == orgId);

            if (role == null)
                throw new NotFoundException("Role not found");

            var existingPermissions = await _db.RolePermissions
                .Where(rp => rp.RoleId == dto.RoleId)
                .ToListAsync();

            _db.RolePermissions.RemoveRange(existingPermissions);

            var permissions = await _db.Permissions
                .Where(p => dto.Permissions.Contains(p.Name))
                .ToListAsync();

            var rolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            });

            _db.RolePermissions.AddRange(rolePermissions);
            await _db.SaveChangesAsync();

            var userIds = await _db.UserRoles
                .Where(ur => ur.RoleId == dto.RoleId && ur.OrganizationId == orgId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            foreach (var userId in userIds)
            {
                await _cache.RemoveAsync($"permissions:{userId}:{orgId}");
            }
        }

        public async Task AssignRoleToUserAsync(AssignRoleToUserDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var roleExists = await _db.Roles
                .AnyAsync(r => r.Id == dto.RoleId && r.OrganizationId == orgId);

            if (!roleExists)
                throw new NotFoundException("Role not found");

            var userInOrg = await _db.OrganizationUsers
                .AnyAsync(ou => ou.UserId == dto.UserId && ou.OrganizationId == orgId);

            if (!userInOrg)
                throw new BadRequestException("User is not a member of this organization");

            var exists = await _db.UserRoles.AnyAsync(x =>
                x.UserId == dto.UserId &&
                x.RoleId == dto.RoleId &&
                x.OrganizationId == orgId);

            if (exists)
                return;

            _db.UserRoles.Add(new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId,
                OrganizationId = orgId
            });

            await _db.SaveChangesAsync();
            await _cache.RemoveAsync($"permissions:{dto.UserId}:{orgId}");
        }

        public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var userRole = await _db.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.OrganizationId == orgId);

            if (userRole == null)
                throw new NotFoundException("User role assignment not found");

            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
            await _cache.RemoveAsync($"permissions:{userId}:{orgId}");
        }

        public async Task<ApiResponse<List<RoleDto>>> GetRolesAsync()
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var roles = await _db.Roles
                .Where(r => r.OrganizationId == orgId)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();

            return ApiResponse<List<RoleDto>>.SuccessResponse(roles);
        }

        public async Task<object> GetRoleWithPermissionsAsync(Guid roleId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var role = await _db.Roles
                .Where(r => r.Id == roleId && r.OrganizationId == orgId)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    Permissions = _db.RolePermissions
                        .Where(rp => rp.RoleId == r.Id)
                        .Join(_db.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (role == null)
                throw new NotFoundException("Role not found");

            return role;
        }
    }
}