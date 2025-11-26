using TodoApi.Domain.Interfaces;

namespace TodoApi.Domain.Domain;

public class Todo : ISoftDeletable
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public long TodoListId { get; set; }
    public TodoList? TodoList { get; set; }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }

    public void MarkAsIncomplete()
    {
        IsCompleted = false;
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
    }
}
