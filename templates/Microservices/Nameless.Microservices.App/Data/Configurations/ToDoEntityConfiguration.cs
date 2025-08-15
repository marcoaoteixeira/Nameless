using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Microservices.App.Domains.Entities;

namespace Nameless.Microservices.App.Data.Configurations;

/// <summary>
///     Entity type configuration for the To-Do entity.
/// </summary>
public class ToDoEntityTypeConfiguration : IEntityTypeConfiguration<ToDo> {
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ToDo> builder) {
        builder.ToTable("todos");

        builder
            .Property(entity => entity.Id)
            .HasColumnName(nameof(ToDo.Id).ToSnakeCase())
            .ValueGeneratedOnAdd();

        builder
            .HasKey(entity => entity.Id)
            .HasName("pk_todos");

        builder
            .Property(entity => entity.Summary)
            .HasColumnName(nameof(ToDo.Summary).ToSnakeCase())
            .HasMaxLength(4096);

        builder
            .Property(entity => entity.DueDate)
            .HasColumnName(nameof(ToDo.DueDate).ToSnakeCase());

        builder
            .Property(entity => entity.ConclusionDate)
            .HasColumnName(nameof(ToDo.ConclusionDate).ToSnakeCase());
    }
}
