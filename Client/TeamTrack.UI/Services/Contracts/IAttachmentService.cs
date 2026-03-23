using TeamTrack.UI.Models.Common;
using TeamTrack.UI.Models.Attachments;

namespace TeamTrack.UI.Services.Contracts;

public interface IAttachmentService
{
    Task<ApiResponse<AttachmentDto>> UploadAsync(MultipartFormDataContent content, Guid? taskId);
    Task<ApiResponse<List<AttachmentDto>>> GetByTaskAsync(Guid taskId);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}