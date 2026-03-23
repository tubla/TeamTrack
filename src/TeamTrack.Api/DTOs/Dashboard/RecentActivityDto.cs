namespace TeamTrack.Api.DTOs.Dashboard;

public class RecentActivityDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = default!;
    public string EntityType { get; set; } = default!;
    public Guid EntityId { get; set; }
    public string? Metadata { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string UserName { get; set; } = default!;
}
