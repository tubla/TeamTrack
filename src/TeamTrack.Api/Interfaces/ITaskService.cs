using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Task;

namespace TeamTrack.Api.Interfaces
{
    public interface ITaskService
    {
        Task<object> CreateAsync(CreateTaskDto dto);
        Task<object> GetAsync(QueryParams param, Guid projectId);
        Task<object> GetByIdAsync(Guid id);
        Task<object> GetMyTasksAsync(QueryParams param);
        Task<object> UpdateAsync(Guid id, UpdateTaskDto dto);
        Task UpdateStatusAsync(UpdateTaskStatusDto dto);
        Task AssignTaskAsync(Guid id, AssignTaskDto dto);
        Task DeleteAsync(Guid id);
    }
}
