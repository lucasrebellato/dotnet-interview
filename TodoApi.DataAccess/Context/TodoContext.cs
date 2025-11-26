using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TodoApi.DataAccess.Context.EntityConfigurations;
using TodoApi.Domain.Domain;
using TodoApi.Domain.Interfaces;

public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoLists { get; set; } = default!;
    public DbSet<Todo> Todos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new TodoListConfiguration());
        modelBuilder.ApplyConfiguration(new TodoConfiguration());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var propertyMethodInfo = typeof(EF).GetMethod("Property")!
                    .MakeGenericMethod(typeof(bool));

                var isDeletedProperty = Expression.Call(
                    propertyMethodInfo,
                    parameter,
                    Expression.Constant("IsDeleted"));

                var compareExpression = Expression.Equal(
                    isDeletedProperty,
                    Expression.Constant(false));

                var lambda = Expression.Lambda(compareExpression, parameter);

                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(lambda);
            }
        }
    }   
}
