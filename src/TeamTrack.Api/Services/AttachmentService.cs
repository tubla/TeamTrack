using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
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
            var url = await _fileService.UploadAsync(file);

            var attachment = new Attachment
            {
                FileName = file.FileName,
                FileUrl = url,
                ContentType = file.ContentType,
                UploadedByUserId = _context.UserId,
                TaskId = taskId
            };

            _db.Attachments.Add(attachment);
            await _db.SaveChangesAsync();

            return new { attachment.Id, attachment.FileUrl };
        }

        public async Task<object> GetByTaskAsync(Guid taskId)
        {
            var items = await _db.Attachments
                .Where(a => a.TaskId == taskId)
                .Select(a => new
                {
                    a.Id,
                    a.FileName,
                    a.FileUrl
                }).ToListAsync();

            return items;
        }
    }
}
