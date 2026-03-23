using TeamTrack.Api.DTOs.Dashboard;

namespace TeamTrack.Api.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}