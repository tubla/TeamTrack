namespace TeamTrack.Api.Interfaces
{
    public interface IAttachmentService
    {
        Task<object> UploadAsync(IFormFile file, Guid? taskId);
        Task<object> GetByTaskAsync(Guid taskId);
    }
}
