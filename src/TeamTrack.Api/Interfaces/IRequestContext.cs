namespace TeamTrack.Api.Interfaces
{
    public interface IRequestContext
    {
        Guid UserId { get; }
        Guid? OrganizationId { get; }
        string Email { get; }
    }
}