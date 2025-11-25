namespace TodoApi.IBusinessLogic.INotifier;
public interface ITodoNotifier
{
    Task NotifyTodoCompleted(long todoId);
    Task NotifyAllCompleted(long todoListId);
}
