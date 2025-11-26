using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Dtos.Request;

namespace TodoApi.BusinessLogic.Mappers.DtoToObject;

public class DtoToTodo
{
    public static Todo Map(CreateTodoDto dto, long listId)
    {
        return new Todo
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            TodoListId = listId
        };
    }
}
