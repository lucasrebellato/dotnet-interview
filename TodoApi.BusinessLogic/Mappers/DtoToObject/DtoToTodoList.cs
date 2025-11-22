using TodoApi.Domain.Domain;
using TodoApi.IBusinessLogic.Dtos.Request;

namespace TodoApi.BusinessLogic.Mappers.DtoToObject;

public class DtoToTodoList
{
    public static TodoList Map(CreateTodoListDto dto)
    {
        return new TodoList
        {
            Name = dto.Name
        };
    }
}
