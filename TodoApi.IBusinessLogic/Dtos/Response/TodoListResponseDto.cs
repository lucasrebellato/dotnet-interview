namespace TodoApi.IBusinessLogic.Dtos.Response;

public class TodoListResponseDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<TodoResponseDto>? Todos { get; set; }
    public bool AllTodosCompleted { get; set; } = false;
}
