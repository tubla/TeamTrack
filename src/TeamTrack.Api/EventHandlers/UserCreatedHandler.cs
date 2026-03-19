using TeamTrack.Api.Events;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.EventHandlers
{
    public class UserCreatedHandler(
        IBackgroundTaskQueue queue,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UserCreatedHandler> logger) : IDomainEventHandler<UserCreatedEvent>
    {
        private readonly IBackgroundTaskQueue _queue = queue;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<UserCreatedHandler> _logger = logger;

        public Task Handle(UserCreatedEvent domainEvent)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
            _queue.Queue(async token =>
            {
                _logger.LogInformation("Processing user created in background: {UserId}| CorrelationId: {CorrelationId}", domainEvent.UserId,correlationId);

                // simulate work
                await Task.Delay(2000, token);

                // future:
                // send email
                // assign default role
            });

            return Task.CompletedTask;
        }
    }
}
