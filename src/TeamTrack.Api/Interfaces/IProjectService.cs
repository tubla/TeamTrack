using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface IProjectService
    {
        Task<object> CreateAsync(CreateProjectDto dto);
        Task<object> GetAsync(QueryParams param);
    }
}
