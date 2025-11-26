using Microsoft.AspNetCore.SignalR;
using TodoApi.Hubs;
using TodoApi.IBusinessLogic.INotifier;

namespace TodoApi.Infrastructure.Notifiers;

public class TodoNotifier : ITodoNotifier
{
    private readonly IHubContext<TodoHub> _hub;

    public TodoNotifier(IHubContext<TodoHub> hub)
    {
        _hub = hub;
    }

    public Task NotifyTodoCompleted(long todoId)
    {
        return _hub.Clients.All.SendAsync("TodoCompleted", todoId);
    }

    public Task NotifyAllCompleted(long todoListId)
    {
        return _hub.Clients.All.SendAsync("TodoCompletedAll", todoListId);
    }
}
