using TeamTrack.UI.Models.Common;
using TeamTrack.UI.Models.Comments;

namespace TeamTrack.UI.Services.Contracts;

public interface ICommentService
{
    Task<ApiResponse<CommentDto>> CreateCommentAsync(CreateCommentRequest request);
    Task<ApiResponse<List<CommentDto>>> GetCommentsByTaskAsync(Guid taskId);
}