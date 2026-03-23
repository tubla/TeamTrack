using TeamTrack.Api.DTOs.Comment;

namespace TeamTrack.Api.Interfaces
{
    public interface ICommentService
    {
        Task<object> CreateAsync(CreateCommentDto dto);
        Task<object> GetByTaskAsync(Guid taskId);
    }
}
