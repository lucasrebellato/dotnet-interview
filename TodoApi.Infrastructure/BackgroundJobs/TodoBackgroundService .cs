using Microsoft.Extensions.DependencyInjection;
using TodoApi.IBusinessLogic.IServices;

namespace TodoApi.Infrastructure.BackgroundJobs;

public class TodoBackgroundService : IBackgroundJobService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceProvider _serviceProvider;

    public TodoBackgroundService(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
    {
        _taskQueue = taskQueue;
        _serviceProvider = serviceProvider;
    }

    public void EnqueueMarkAllTodosCompleted(long todoListId)
    {
        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            using var scope = _serviceProvider.CreateScope();
            var todoService = scope.ServiceProvider.GetRequiredService<ITodoService>();
            await todoService.MarkAllAsCompleted(todoListId);
        });
    }
}
