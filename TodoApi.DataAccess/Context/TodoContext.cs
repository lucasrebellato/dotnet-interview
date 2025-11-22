using Microsoft.EntityFrameworkCore;
using TodoApi.DataAccess.Context.EntityConfigurations;
using TodoApi.Domain.Domain;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoLists { get; set; } = default!;
    public DbSet<Todo> Todos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoListConfiguration());
    }   
}
