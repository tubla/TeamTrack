using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class LocalFileService(IWebHostEnvironment env) : IFileService
    {
        private readonly IWebHostEnvironment _env = env;

        public async Task<string> UploadAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/{fileName}";
        }
    }
}
