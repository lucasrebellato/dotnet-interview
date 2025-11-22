using TodoApi.Domain.Interfaces;

namespace TodoApi.Domain.Domain;

public class Todo : ISoftDeletable
{
    public long Id { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public long TodoListId { get; set; }
}
