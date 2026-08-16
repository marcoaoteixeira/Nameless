using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Nameless.EventSourcing.EntityFrameworkCore;

/// <summary>
///     Maps <see cref="EventEnvelope"/> to the <c>event_store</c> table.
///     Applied by the consuming application's own
///     <see cref="Microsoft.EntityFrameworkCore.DbContext"/> in
///     <c>OnModelCreating</c>, alongside a <c>DbSet&lt;EventEnvelope&gt;</c>.
/// </summary>
public sealed class EventEnvelopeEntityTypeConfiguration : IEntityTypeConfiguration<EventEnvelope> {
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<EventEnvelope> builder) {
        const string TableName = "event_store";

        builder.ToTable(TableName);

        builder
            .HasKey(keyExpression: envelope => envelope.EventID)
            .HasName($"PK_{TableName}");

        builder
            .Property(propertyExpression: envelope => envelope.StreamID)
            .IsRequired();

        builder
            .Property(propertyExpression: envelope => envelope.AggregateType)
            .IsRequired();

        builder
            .Property(propertyExpression: envelope => envelope.EventType)
            .IsRequired();

        builder
            .Property(propertyExpression: envelope => envelope.Payload)
            .HasColumnType(typeName: "jsonb")
            .IsRequired();

        builder
            .Property(propertyExpression: envelope => envelope.GlobalSequence)
            .ValueGeneratedOnAdd();

        // Optimistic concurrency: a stream cannot have two events at the
        // same version.
        builder
            .HasIndex(indexExpression: envelope => new { envelope.StreamID, envelope.Version })
            .HasDatabaseName($"IX_{TableName}_{nameof(EventEnvelope.StreamID)}_{nameof(EventEnvelope.Version)}")
            .IsUnique();

        builder
            .HasIndex(indexExpression: envelope => envelope.TenantID)
            .HasDatabaseName($"IX_{TableName}_{nameof(EventEnvelope.TenantID)}");

        builder
            .HasIndex(indexExpression: envelope => envelope.GlobalSequence)
            .HasDatabaseName($"IX_{TableName}_{nameof(EventEnvelope.GlobalSequence)}");
    }
}