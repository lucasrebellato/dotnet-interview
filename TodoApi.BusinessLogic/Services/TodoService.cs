using Microsoft.AspNetCore.SignalR;
using TodoApi.BusinessLogic.Interfaces;
using TodoApi.BusinessLogic.Mappers.DtoToObject;
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

        Todo todo = DtoToTodo.Map(dto, todoListId);

        await _todoRepository.Add(todo);

        return TodoToDto.Map(todo);
    }

    public async Task Delete(long todoListId, long id)
    {
        Todo todo = await GetTodoFromListOrThrow(todoListId, id);

        await _todoRepository.Delete(todo);
    }

    public async Task<TodoResponseDto> GetById(long todoListId, long id)
    {
        Todo todo = await GetTodoFromListOrThrow(todoListId, id);

        return TodoToDto.Map(todo!);
    }

    public async Task MarkAllAsCompleted(long todoListId)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        List<Todo> todosToUpdate = todoList.Todos.Where(t => !t.IsCompleted).ToList();

        foreach (Todo todo in todosToUpdate)
        {
            todo.MarkAsCompleted();
            await _notifier.NotifyTodoCompleted(todo.Id);
        }

        await _todoRepository.SaveChangesAsync();
        await _notifier.NotifyAllCompleted(todoListId);
    }

    public async Task MarkAsCompleted(long todoListId, long id)
    {
        Todo todo = await GetTodoFromListOrThrow(todoListId, id);

        todo!.MarkAsCompleted();

        await _todoRepository.Update(todo);
    }

    public async Task MarkAsIncompleted(long todoListId, long id)
    {
        Todo todo = await GetTodoFromListOrThrow(todoListId, id);

        todo!.MarkAsIncomplete();

        await _todoRepository.Update(todo);
    }

    public async Task<TodoResponseDto> Update(long todoListId, long id, UpdateTodoDto dto)
    {
        Todo todo = await GetTodoFromListOrThrow(todoListId, id);

        todo!.Update(dto.Title, dto.Description);

        await _todoRepository.Update(todo);

        return TodoToDto.Map(todo);
    }

    private async Task<Todo> GetTodoFromListOrThrow(long todoListId, long id)
    {
        TodoList todoList = await _todoListService.GetByIdWithIncludes(todoListId, ["Todos"]);

        Todo? todo = todoList.Todos.FirstOrDefault(t => t.Id == id);

        Utils<Todo>.CheckForNullValue(todo);

        return todo!;
    }
}
