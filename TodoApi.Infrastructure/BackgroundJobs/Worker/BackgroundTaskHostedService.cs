using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TodoApi.Infrastructure.BackgroundJobs.Worker;

public class BackgroundTaskHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<BackgroundTaskHostedService> _logger;

    public BackgroundTaskHostedService(
        IBackgroundTaskQueue taskQueue,
        ILogger<BackgroundTaskHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BackgroundTaskHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing background task.");
            }
        }

        _logger.LogInformation("BackgroundTaskHostedService stopped.");
    }
}
