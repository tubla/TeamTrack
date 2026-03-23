namespace TeamTrack.Api.DTOs.Permission;

public class PermissionGroupDto
{
    public string GroupName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = [];
}
