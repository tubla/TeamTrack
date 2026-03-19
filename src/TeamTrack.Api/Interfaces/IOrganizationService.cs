using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface IOrganizationService
    {
        Task<object> CreateAsync(CreateOrganizationDto dto);
    }
}