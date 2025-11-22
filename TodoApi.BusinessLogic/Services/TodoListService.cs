using TodoApi.BusinessLogic.Mappers.DtoToObject;
using TodoApi.BusinessLogic.Mappers.ObjectToDto;
using TodoApi.BusinessLogic.Utils;
using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.Interfaces;
using TodoApi.IDataAccess;

namespace TodoApi.BusinessLogic.Services;

public class TodoListService : ITodoListService
{
    private readonly IGenericRepository<TodoList> _todoListRepository;

    public TodoListService(IGenericRepository<TodoList> todoListRepository)
    {
        _todoListRepository = todoListRepository;
    }

    public async Task<TodoListResponseDto> Create(CreateTodoListDto dto)
    {
        TodoList todoList = DtoToTodoList.Map(dto);

        await _todoListRepository.Add(todoList);

        return TodoListToDto.Map(todoList);
    }

    public async Task Delete(long id)
    {
        TodoList todoList = await _todoListRepository.Get(x => x.Id == id, []);

        Utils<TodoList>.CheckForNullValue(todoList);

        await _todoListRepository.Delete(todoList);
    }

    public async Task<List<TodoListResponseDto>> GetAll()
    {
        List<TodoList> todoLists = await _todoListRepository.GetAll(t => true, []);

        List<TodoListResponseDto> listResponse = todoLists.Select(TodoListToDto.Map).ToList();

        return listResponse;
    }

    public async Task<TodoListResponseDto> GetById(long id)
    {
        TodoList todoList = await _todoListRepository.Get(x => x.Id == id, ["Todos"]);

        Utils<TodoList>.CheckForNullValue(todoList);

        return TodoListToDto.Map(todoList);
    }

    public async Task<TodoListResponseDto> Update(long id, UpdateTodoListDto dto)
    {
        TodoList todoList = await _todoListRepository.Get(x => x.Id == id, ["Todos"]);

        Utils<TodoList>.CheckForNullValue(todoList);

        todoList.Name = dto.Name;
        await _todoListRepository.Update(todoList);

        return TodoListToDto.Map(todoList);
    }
}
