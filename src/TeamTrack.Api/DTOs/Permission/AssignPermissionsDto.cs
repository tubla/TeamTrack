namespace TeamTrack.Api.DTOs.Permission;

public class AssignPermissionsDto
{
    public Guid RoleId { get; set; }
    public List<string> Permissions { get; set; } = [];
}
