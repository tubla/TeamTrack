using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.Project;

namespace TeamTrack.Api.Interfaces
{
    public interface IProjectService
    {
        Task<object> CreateAsync(CreateProjectDto dto);
        Task<object> GetAsync(QueryParams param);
        Task<object> GetByIdAsync(Guid id);
        Task<object> UpdateAsync(Guid id, UpdateProjectDto dto);
        Task DeleteAsync(Guid id);
    }
}
