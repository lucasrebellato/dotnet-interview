using Microsoft.AspNetCore.SignalR;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.BusinessLogic.Mappers.ObjectToDto;
using TodoApi.BusinessLogic.Utils;
using TodoApi.Domain.Domain;
using TodoApi.Hubs;
using TodoApi.IBusinessLogic.Dtos.Request;
using TodoApi.IBusinessLogic.Dtos.Response;
using TodoApi.IBusinessLogic.INotifier;
using TodoApi.IBusinessLogic.IServices;
using TodoApi.IDataAccess;

namespace TodoApi.BusinessLogic.Services;

public class TodoService : ITodoService
{
    private readonly IGenericRepository<Todo> _todoRepository;
    private readonly ITodoListInternalService _todoListService;
    private readonly ITodoNotifier _notifier;

    public TodoService(IGenericRepository<Todo> todoRepository, ITodoListInternalService todoListService, ITodoNotifier notifier)
    {
        _todoRepository = todoRepository;
        _todoListService = todoListService;
        _notifier = notifier;
    }

    public async Task<TodoResponseDto> Create(long todoListId, CreateTodoDto dto)
    {
        await _todoListService.Exists(todoListId);

        var todo = new Todo
        {
            Description = dto.Description,
            TodoListId = todoListId
        };

        await _todoRepository.Add(todo);

        return TodoToDto.Map(todo);
    }

    public async Task Delete(long id)
    {
        Todo todo = await _todoRepository.Get(x => x.Id == id, []);

        Utils<Todo>.CheckForNullValue(todo);

        await _todoRepository.Delete(todo);
    }

    public async Task<TodoResponseDto> GetById(long todoListId, long id)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        Todo todo = todoList.Todos.FirstOrDefault(t => t.Id == id);
        Utils<Todo>.CheckForNullValue(todo);

        return TodoToDto.Map(todo);
    }

    public async Task MarkAllAsCompleted(long todoListId)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        List<Todo> todosToUpdate = todoList.Todos.Where(t => !t.IsCompleted).ToList();

        foreach (Todo todo in todosToUpdate)
        {
            todo.IsCompleted = true;

            await _notifier.NotifyTodoCompleted(todo.Id);
        }

        await _todoRepository.SaveChangesAsync();
        await _notifier.NotifyAllCompleted(todoListId);
    }

    public async Task MarkAsCompleted(long todoListId, long id)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        Todo todo = todoList.Todos.FirstOrDefault(t => t.Id == id);
        Utils<Todo>.CheckForNullValue(todo);

        todo.IsCompleted = true;

        await _todoRepository.Update(todo);
    }

    public async Task<TodoResponseDto> Update(long todoListId, long id, UpdateTodoDto dto)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        Todo todo = todoList.Todos.FirstOrDefault(t => t.Id == id);
        Utils<Todo>.CheckForNullValue(todo);

        todo.Description = dto.Description;
        
        await _todoRepository.Update(todo);
        
        return TodoToDto.Map(todo);
    }
}
