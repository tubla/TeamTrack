using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class QueuedHostedService(IBackgroundTaskQueue queue, ILogger<QueuedHostedService> logger) : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue = queue;
        private readonly ILogger<QueuedHostedService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing background task");
                }
            }
        }
    }
}