using TeamTrack.Api.Common;

namespace TeamTrack.Api.Interfaces
{
    public interface IDomainEventHandler<T> where T : DomainEvent
    {
        Task Handle(T domainEvent);
    }
}
