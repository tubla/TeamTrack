namespace TeamTrack.Api.DTOs.OrgAccess
{
    public class ApproveOrgAccessRequestDto
    {
        public Guid RequestId { get; set; }
        public List<OrgAssignmentDto> Assignments { get; set; } = [];
    }

}
