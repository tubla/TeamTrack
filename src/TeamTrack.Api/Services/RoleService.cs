using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Extensions;
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
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RoleId);

            if (role == null)
                throw new NotFoundException("Role not found");

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
        }

        public async Task AssignRoleToUserAsync(AssignRoleToUserDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

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
            await _cache.RemoveAsync($"permissions:{dto.UserId}");

        }

        public async Task<object> GetRolesAsync(QueryParams param)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var query = _db.Roles
                .Where(r => r.OrganizationId == orgId);

            query = query
                .ApplySearch(param.Search, r => r.Name)
                .ApplySorting(param.SortBy);

            var total = await query.CountAsync();

            var roles = await query
                .ApplyPaging(param)
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return new PagedResponse<object>
            {
                Items = roles,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }
    }
}