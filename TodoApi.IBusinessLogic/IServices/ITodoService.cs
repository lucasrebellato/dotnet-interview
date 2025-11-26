using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;

namespace TodoApi.IBusinessLogic.IServices;

public interface ITodoService
{
    Task<TodoResponseDto> Create(long todoListId, CreateTodoDto dto);
    Task Delete(long todoListId, long id);
    Task<TodoResponseDto> GetById(long todoListId, long id);
    Task<TodoResponseDto> Update(long todoListId, long id, UpdateTodoDto dto);
    Task MarkAsCompleted(long todoListId, long id);
    Task MarkAsIncompleted(long todoListId, long id);
    Task MarkAllAsCompleted(long todoListId);
}
