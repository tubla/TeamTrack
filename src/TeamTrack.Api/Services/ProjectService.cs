using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Extensions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class ProjectService(ApplicationDbContext db, IRequestContext context) : IProjectService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IRequestContext _context = context;

        public async Task<object> CreateAsync(CreateProjectDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                OrganizationId = orgId,
                CreatedByUserId = _context.UserId
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return new { project.Id, project.Name };
        }

        public async Task<object> GetAsync(QueryParams param)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var query = _db.Projects
                .Where(p => p.OrganizationId == orgId);

            var total = await EntityFrameworkQueryableExtensions.CountAsync(query);

            var itemsQuery = query
                .ApplyPaging(param)
                .Select(p => new
                {
                    p.Id,
                    p.Name
                });

            var items = await EntityFrameworkQueryableExtensions
                .ToListAsync(itemsQuery);

            return new PagedResponse<object>
            {
                Items = items,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }
    }
}
