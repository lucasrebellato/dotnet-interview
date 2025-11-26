using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Dtos.Response;

namespace TodoApi.BusinessLogic.Mappers.ObjectToDto;

public static class TodoListToDto
{
    public static TodoListResponseDto Map(TodoList todoList)
    {
        return new TodoListResponseDto
        {
            Id = todoList.Id,
            Name = todoList.Name,
            Todos = todoList.Todos?.Select(t => TodoToDto.Map(t)).ToList(),
            AllTodosCompleted = todoList.Todos!.Any() && todoList.Todos!.All(t => t.IsCompleted)
        };
    }
}
