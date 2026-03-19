using TeamTrack.Api.Common;

namespace TeamTrack.Api.Events
{
    public class UserCreatedEvent(Guid userId) : DomainEvent
    {
        public Guid UserId { get; } = userId;
    }
}
