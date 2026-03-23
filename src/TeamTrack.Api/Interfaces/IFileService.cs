namespace TeamTrack.Api.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file);
        Task DeleteAsync(string fileUrl);
    }
}
