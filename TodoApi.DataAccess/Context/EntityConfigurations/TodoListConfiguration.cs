using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Domain.Domain;

namespace TodoApi.DataAccess.Context.EntityConfigurations;
internal class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.HasKey(tl => tl.Id);

        builder.Property(tl => tl.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasMany(tl => tl.Todos)
            .WithOne()
            .HasForeignKey(t => t.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
