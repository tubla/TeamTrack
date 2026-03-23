using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Project;
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

            var query = _db.Projects.Where(p => p.OrganizationId == orgId);

            var total = await query.CountAsync();

            var items = await query
                .ApplyPaging(param)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CreatedAt
                })
                .ToListAsync();

            return new PagedResponse<object>
            {
                Items = items,
                Page = param.Page,
                PageSize = param.PageSize,
                TotalCount = total
            };
        }

        public async Task<object> GetByIdAsync(Guid id)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var project = await _db.Projects
                .Where(p => p.Id == id && p.OrganizationId == orgId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Description,
                    p.CreatedAt,
                    TaskCount = _db.Tasks.Count(t => t.ProjectId == p.Id)
                })
                .FirstOrDefaultAsync();

            if (project == null)
                throw new NotFoundException("Project not found");

            return project;
        }

        public async Task<object> UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);

            if (project == null)
                throw new NotFoundException("Project not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                project.Name = dto.Name;

            if (dto.Description != null)
                project.Description = dto.Description;

            await _db.SaveChangesAsync();

            return new { project.Id, project.Name };
        }

        public async Task DeleteAsync(Guid id)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var project = await _db.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);

            if (project == null)
                throw new NotFoundException("Project not found");

            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
        }
    }
}
