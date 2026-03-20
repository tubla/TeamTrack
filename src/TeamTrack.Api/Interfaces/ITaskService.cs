using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface ITaskService
    {
        Task<object> CreateAsync(CreateTaskDto dto);
        Task<object> GetAsync(QueryParams param, Guid projectId);
        Task UpdateStatusAsync(UpdateTaskStatusDto dto);
    }
}
