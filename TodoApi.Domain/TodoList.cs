namespace TodoApi.Domain;

public class TodoList
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<Todo> Todos { get; set; } = [];
}
