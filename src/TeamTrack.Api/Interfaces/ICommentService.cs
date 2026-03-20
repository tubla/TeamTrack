using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface ICommentService
    {
        Task<object> CreateAsync(CreateCommentDto dto);
        Task<object> GetByTaskAsync(Guid taskId);
    }
}
