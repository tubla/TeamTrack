using TeamTrack.Api.Common;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Events
{
    public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task DispatchAsync(DomainEvent domainEvent)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod("Handle");
                await (Task)method!.Invoke(handler, new object[] { domainEvent })!;
            }
        }
    }
}
