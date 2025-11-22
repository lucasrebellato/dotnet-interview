using Microsoft.EntityFrameworkCore;

namespace TodoApi.DataAccess.Context;

public class InMemoryTodoContextFactory
{
    public static TodoContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TodoContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
