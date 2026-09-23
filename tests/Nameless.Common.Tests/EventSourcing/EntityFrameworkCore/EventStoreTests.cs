using Microsoft.EntityFrameworkCore;
using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.EntityFrameworkCore;

[IntegrationTest]
public class EventStoreTests {
    [EventType("test.event", 1)]
    private sealed record SerializableTestEvent(string Name) : IEvent;

    private sealed class TestDbContext : DbContext {
        public DbSet<EventEnvelope> Events => Set<EventEnvelope>();

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfiguration(new EventEnvelopeEntityTypeConfiguration());

            // SQLite has no equivalent of Postgres' serial/identity columns
            // for a non-key column, so GlobalSequence's ValueGeneratedOnAdd
            // needs a default + trigger here to behave the same way in
            // tests. Production code targeting SQLite hits the same gap.
            modelBuilder.Entity<EventEnvelope>()
                        .Property(e => e.GlobalSequence)
                        .HasDefaultValue(0L);
        }
    }

    private static TestDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new TestDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        context.Database.ExecuteSqlRaw("""
            CREATE TRIGGER trg_event_store_global_sequence
            AFTER INSERT ON event_store
            WHEN NEW.GlobalSequence = 0
            BEGIN
                UPDATE event_store
                SET GlobalSequence = (SELECT IFNULL(MAX(GlobalSequence), 0) + 1 FROM event_store)
                WHERE EventID = NEW.EventID;
            END;
            """);

        return context;
    }

    private static EventSerializer CreateSerializer() {
        var catalog = new EventTypeCatalog([typeof(SerializableTestEvent)]);
        return new EventSerializer(catalog, upCasters: []);
    }

    [Fact]
    public async Task AppendAsync_PersistsEvents() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        var events = new IEvent[] { new SerializableTestEvent("Alpha"), new SerializableTestEvent("Beta") };

        // act
        await sut.AppendAsync(
            streamID: "Test-1",
            aggregateType: "Test",
            aggregateID: Guid.NewGuid(),
            tenantID: null,
            expectedVersion: 0,
            events: events,
            causedBy: Guid.NewGuid(),
            correlationID: Guid.NewGuid(),
            TestContext.Current.CancellationToken
        );

        // assert
        var stream = await sut.ReadStreamAsync("Test-1", TestContext.Current.CancellationToken);
        Assert.Equal(2, stream.Count);
    }

    [Fact]
    public async Task AppendAsync_WithEmptyEvents_DoesNothing() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        // act
        await sut.AppendAsync(
            "Test-1", "Test", Guid.NewGuid(), null, 0,
            events: [], Guid.NewGuid(), Guid.NewGuid(),
            TestContext.Current.CancellationToken
        );

        // assert
        var stream = await sut.ReadStreamAsync("Test-1", TestContext.Current.CancellationToken);
        Assert.Empty(stream);
    }

    [Fact]
    public async Task AppendAsync_VersionConflict_ThrowsConcurrencyConflictException() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        var aggregateID = Guid.NewGuid();

        await sut.AppendAsync(
            "Test-1", "Test", aggregateID, null, 0,
            [new SerializableTestEvent("Alpha")], Guid.NewGuid(), Guid.NewGuid(),
            TestContext.Current.CancellationToken
        );

        // act & assert — appending again with the same expectedVersion
        // produces the same Version=1 row, violating the unique index.
        await Assert.ThrowsAsync<ConcurrencyConflictException>(
            () => sut.AppendAsync(
                "Test-1", "Test", aggregateID, null, 0,
                [new SerializableTestEvent("Beta")], Guid.NewGuid(), Guid.NewGuid(),
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task AppendAsync_WithNullOrWhiteSpaceStreamID_Throws() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => sut.AppendAsync(
                " ", "Test", Guid.NewGuid(), null, 0,
                [new SerializableTestEvent("Alpha")], Guid.NewGuid(), Guid.NewGuid(),
                TestContext.Current.CancellationToken
            )
        );
    }

    [Fact]
    public async Task ReadStreamAsync_ReturnsEventsOrderedByVersion() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        var aggregateID = Guid.NewGuid();

        await sut.AppendAsync("Test-1", "Test", aggregateID, null, 0,
            [new SerializableTestEvent("First"), new SerializableTestEvent("Second")],
            Guid.NewGuid(), Guid.NewGuid(), TestContext.Current.CancellationToken);

        // act
        var stream = await sut.ReadStreamAsync("Test-1", TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal(2, stream.Count),
            () => Assert.Equal(1, stream[0].Version),
            () => Assert.Equal(2, stream[1].Version)
        );
    }

    [Fact]
    public async Task GetAsync_ReturnsEventsAfterGivenGlobalSequence() {
        // arrange
        await using var dbContext = CreateDbContext();
        var sut = new EventStore<TestDbContext>(dbContext, CreateSerializer());

        await sut.AppendAsync("Test-1", "Test", Guid.NewGuid(), null, 0,
            [new SerializableTestEvent("First")], Guid.NewGuid(), Guid.NewGuid(), TestContext.Current.CancellationToken);
        await sut.AppendAsync("Test-2", "Test", Guid.NewGuid(), null, 0,
            [new SerializableTestEvent("Second")], Guid.NewGuid(), Guid.NewGuid(), TestContext.Current.CancellationToken);

        // act
        var all = new List<EventEnvelope>();
        await foreach (var envelope in sut.GetAsync(0, TestContext.Current.CancellationToken)) {
            all.Add(envelope);
        }

        // assert
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public void Constructor_WithNullDbContext_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new EventStore<TestDbContext>(null!, CreateSerializer()));
    }
}
