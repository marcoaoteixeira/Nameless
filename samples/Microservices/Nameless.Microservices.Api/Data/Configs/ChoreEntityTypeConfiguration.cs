using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.Microservices.Api.Domains.Chores.Entities;

namespace Nameless.Microservices.Api.Data.Configs;

public class ChoreEntityTypeConfiguration : IEntityTypeConfiguration<Chore> {
    public void Configure(EntityTypeBuilder<Chore> builder) {
        const string TableName = "chores";

        builder.ToTable(TableName);

        builder
            .HasKey(entity => entity.ID)
            .HasName($"PK_{TableName}");

        builder
            .Property(entity => entity.Title)
            .IsRequired();

        builder
            .Property(entity => entity.Description)
            .IsRequired();

        builder
            .Property(entity => entity.DueDate);

        builder
            .Property(entity => entity.ConclusionDate);
    }
}
