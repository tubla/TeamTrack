using TeamTrack.Api.Common;

namespace TeamTrack.Api.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(DomainEvent domainEvent);
    }
}
