using Microsoft.EntityFrameworkCore;

namespace Nameless.EventSourcing.EntityFrameworkCore;

[UnitTest]
public class EventEnvelopeEntityTypeConfigurationTests {
    [Fact]
    public void Configure_MapsTableKeyAndIndexes() {
        // arrange
        var modelBuilder = new ModelBuilder();

        // act
        modelBuilder.ApplyConfiguration(new EventEnvelopeEntityTypeConfiguration());

        var entityType = modelBuilder.Model.FindEntityType(typeof(EventEnvelope));

        // assert
        Assert.Multiple(
            () => Assert.NotNull(entityType),
            () => Assert.Equal("event_store", entityType!.GetTableName()),
            () => Assert.Equal(
                [nameof(EventEnvelope.EventID)],
                entityType!.FindPrimaryKey()!.Properties.Select(p => p.Name)
            ),
            () => Assert.False(entityType!.FindProperty(nameof(EventEnvelope.StreamID))!.IsNullable),
            () => Assert.False(entityType!.FindProperty(nameof(EventEnvelope.AggregateType))!.IsNullable),
            () => Assert.False(entityType!.FindProperty(nameof(EventEnvelope.EventType))!.IsNullable),
            () => Assert.False(entityType!.FindProperty(nameof(EventEnvelope.Payload))!.IsNullable),
            () => Assert.Equal("jsonb", entityType!.FindProperty(nameof(EventEnvelope.Payload))!.GetColumnType()),
            () => Assert.Equal(3, entityType!.GetIndexes().Count())
        );
    }

    [Fact]
    public void Configure_StreamIDAndVersionIndex_IsUnique() {
        // arrange
        var modelBuilder = new ModelBuilder();
        modelBuilder.ApplyConfiguration(new EventEnvelopeEntityTypeConfiguration());

        var entityType = modelBuilder.Model.FindEntityType(typeof(EventEnvelope))!;

        // act
        var index = entityType.GetIndexes()
            .Single(i => i.Properties.Select(p => p.Name)
                          .SequenceEqual([nameof(EventEnvelope.StreamID), nameof(EventEnvelope.Version)]));

        // assert
        Assert.True(index.IsUnique);
    }
}
