namespace TodoApi.IBusinessLogic.IServices;
public interface IBackgroundJobService
{
    void EnqueueMarkAllTodosCompleted(long todoListId);
}
