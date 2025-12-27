using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Microservices.App.Entities;

namespace Nameless.Microservices.App.Data.Configurations;

/// <summary>
///     Entity type configuration for the To-Do entity.
/// </summary>
public class ToDoItemEntityTypeConfiguration : IEntityTypeConfiguration<ToDoItem> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ToDoItem> builder) {
        builder.ToTable("todos");

        builder
            .Property(entity => entity.ID)
            .HasColumnName(nameof(ToDoItem.ID).ToSnakeCase())
            .ValueGeneratedOnAdd();

        builder
            .HasKey(entity => entity.ID)
            .HasName("pk_todos");

        builder
            .Property(entity => entity.Summary)
            .HasColumnName(nameof(ToDoItem.Summary).ToSnakeCase())
            .HasMaxLength(4096);

        builder
            .Property(entity => entity.DueDate)
            .HasColumnName(nameof(ToDoItem.DueDate).ToSnakeCase());

        builder
            .Property(entity => entity.ConclusionDate)
            .HasColumnName(nameof(ToDoItem.ConclusionDate).ToSnakeCase());
    }
}
