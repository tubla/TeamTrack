namespace TeamTrack.Api.DTOs.OrgAccess
{
    public class OrgAssignmentDto
    {
        public Guid OrganizationId { get; set; }
        public Guid RoleId { get; set; }  // Provided from roles API
    }

}
