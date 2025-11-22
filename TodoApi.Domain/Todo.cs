namespace TodoApi.Domain;

public class Todo
{
    public long Id { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
}
