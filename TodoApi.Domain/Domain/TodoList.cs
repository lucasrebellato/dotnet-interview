using TodoApi.Domain.Interfaces;

namespace TodoApi.Domain.Domain;

public class TodoList : ISoftDeletable
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<Todo> Todos { get; set; } = [];
    public bool IsDeleted { get; set; } = false;

    public void Update(string name)
    {
        Name = name;
    }
}
