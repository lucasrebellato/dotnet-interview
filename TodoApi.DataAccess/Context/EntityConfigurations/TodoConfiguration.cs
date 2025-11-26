using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApi.Domain.Domain;

namespace TodoApi.DataAccess.Context.EntityConfigurations;

internal class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.IsCompleted)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .IsRequired();

        builder.Property(t => t.TodoListId)
            .IsRequired();
    }
}
