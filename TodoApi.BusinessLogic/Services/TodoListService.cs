using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.Interfaces;

namespace TodoApi.BusinessLogic.Services;

public class TodoListService : ITodoListService
{
    public Task<TodoListResponseDto> Create(CreateTodoListDto dto)
    {
        throw new NotImplementedException();
    }

    public Task Delete(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TodoListResponseDto>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<TodoListResponseDto> GetById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<TodoListResponseDto> Update(long id, UpdateTodoListDto dto)
    {
        throw new NotImplementedException();
    }
}
