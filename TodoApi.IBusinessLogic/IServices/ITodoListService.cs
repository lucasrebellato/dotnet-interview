using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;

namespace TodoApi.IBusinessLogic.Interfaces;

public interface ITodoListService
{
    Task<List<TodoListResponseDto>> GetAll();
    Task<TodoListResponseDto> GetById(long id);
    Task<TodoListResponseDto> Create(CreateTodoListDto dto);
    Task<TodoListResponseDto> Update(long id, UpdateTodoListDto dto);
    Task Delete(long id);
}
