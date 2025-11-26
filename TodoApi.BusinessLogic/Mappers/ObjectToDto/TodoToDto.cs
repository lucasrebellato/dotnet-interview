using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Dtos.Response;

namespace TodoApi.BusinessLogic.Mappers.ObjectToDto;

public static class TodoToDto
{
    public static TodoResponseDto Map(Todo todo)
    {
        return new TodoResponseDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
        };
    }
}
