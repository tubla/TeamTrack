using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TeamTrack.Api.Hubs;

[Authorize]
public class TeamTrackHub : Hub
{
    /// <summary>
    /// Join organization group for real-time updates
    /// </summary>
    public async Task JoinOrganization(string organizationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"org_{organizationId}");
        await Clients.Caller.SendAsync("Joined", $"Connected to organization {organizationId}");
    }

    /// <summary>
    /// Leave organization group
    /// </summary>
    public async Task LeaveOrganization(string organizationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"org_{organizationId}");
    }

    /// <summary>
    /// Join project group for task board updates
    /// </summary>
    public async Task JoinProject(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }

    /// <summary>
    /// Leave project group
    /// </summary>
    public async Task LeaveProject(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
