using TeamTrack.Api.Common;
using TeamTrack.Api.DTOs.OrgAccess;

namespace TeamTrack.Api.Interfaces
{
    public interface IOrgAccessService
    {
        Task<ApiResponse<string>> CreateRequestAsync(OrgAccessRequestDto dto);
        Task<ApiResponse<string>> ApproveRequestAsync(ApproveOrgAccessRequestDto dto);
        Task<ApiResponse<string>> RejectRequestAsync(Guid requestId);
        Task<ApiResponse<List<PendingOrgAccessRequestDto>>> GetPendingRequestsAsync();
    }

}
