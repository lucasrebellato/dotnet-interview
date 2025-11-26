using TodoApi.Domain.Domain;

namespace TodoApi.BusinessLogic.Interfaces;

public interface ITodoListInternalService
{
    Task Exists(long id);
    Task<TodoList> GetByIdWithIncludes(long id, params string[] includes);
}
