using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class PermissionService(ApplicationDbContext db, IRequestContext context) : IPermissionService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;

        public async Task<bool> HasPermission(Guid userId, string permission)
        {
            var orgId = _context.OrganizationId;

            return await _db.UserRoles
                .Where(ur => ur.UserId == userId && ur.OrganizationId == orgId)
                .Join(_db.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_db.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .AnyAsync(p => p.Name == permission);
        }
    }
}