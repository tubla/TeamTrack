namespace TeamTrack.Api.DTOs.Project;

// ===== Project DTOs =====
public class CreateProjectDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
