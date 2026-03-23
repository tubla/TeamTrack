using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
    public class AttachmentService(ApplicationDbContext db, IFileService fileService, IRequestContext context) : IAttachmentService
    {
        private readonly ApplicationDbContext _db = db;
        private readonly IFileService _fileService = fileService;
        private readonly IRequestContext _context = context;

        public async Task<object> UploadAsync(IFormFile file, Guid? taskId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            // If task is specified, validate it belongs to the organization
            if (taskId.HasValue)
            {
                var taskExists = await _db.Tasks
                    .Include(t => t.Project)
                    .AnyAsync(t => t.Id == taskId.Value && t.Project.OrganizationId == orgId);

                if (!taskExists)
                    throw new NotFoundException("Task not found");
            }

            var url = await _fileService.UploadAsync(file);

            var attachment = new Attachment
            {
                FileName = file.FileName,
                FileUrl = url,
                ContentType = file.ContentType,
                UploadedByUserId = _context.UserId,
                OrganizationId = orgId,
                TaskId = taskId
            };

            _db.Attachments.Add(attachment);
            await _db.SaveChangesAsync();

            return new { attachment.Id, attachment.FileUrl, attachment.FileName };
        }

        public async Task<object> GetByTaskAsync(Guid taskId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            // Validate task belongs to organization
            var taskExists = await _db.Tasks
                .Include(t => t.Project)
                .AnyAsync(t => t.Id == taskId && t.Project.OrganizationId == orgId);

            if (!taskExists)
                throw new NotFoundException("Task not found");

            var items = await _db.Attachments
                .Where(a => a.TaskId == taskId && a.OrganizationId == orgId)
                .Select(a => new
                {
                    a.Id,
                    a.FileName,
                    a.FileUrl,
                    a.ContentType,
                    a.CreatedAt
                }).ToListAsync();

            return items;
        }

        public async Task DeleteAsync(Guid attachmentId)
        {
            var orgId = _context.OrganizationId ?? throw new BadRequestException("Organization required");

            var attachment = await _db.Attachments
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.OrganizationId == orgId);

            if (attachment == null)
                throw new NotFoundException("Attachment not found");

            // Delete from storage
            await _fileService.DeleteAsync(attachment.FileUrl);

            _db.Attachments.Remove(attachment);
            await _db.SaveChangesAsync();
        }
    }
}
