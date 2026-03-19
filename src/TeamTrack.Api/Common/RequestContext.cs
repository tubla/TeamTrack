namespace TeamTrack.Api.Common
{
    public class RequestContext
    {
        public Guid UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string Email { get; set; }
    }
}