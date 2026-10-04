using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Nameless.EntityFrameworkCore.Entities;
using EntityState = Nameless.EntityFrameworkCore.Entities.EntityState;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class QueryableExtensionsTests : IDisposable {
    private static readonly DateTimeOffset Now = new(2026, 3, 10, 8, 30, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Earlier = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly SqliteConnection _connection;
    private readonly FakeTimeProvider _timeProvider = new(Now);

    public QueryableExtensionsTests() {
        // a single open connection keeps the in-memory database alive
        // across multiple DbContext instances.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public void Dispose() {
        _connection.Dispose();
    }

    // ---------------------------------------------------------------------------
    // Inline test infrastructure
    // ---------------------------------------------------------------------------

    private sealed class TestEntity : EntityBase {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestDbContext : DbContext {
        public DbSet<TestEntity> TestEntities => Set<TestEntity>();

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<TestEntity>(e => {
                e.HasKey(x => x.ID);
                e.Property(x => x.Name).IsRequired();
            });
        }
    }

    // ---------------------------------------------------------------------------
    // Factory helpers
    // ---------------------------------------------------------------------------

    private TestDbContext CreateContext() {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        return new TestDbContext(options);
    }

    private TestEntity[] Seed(params TestEntity[] entities) {
        using var context = CreateContext();

        context.TestEntities.AddRange(entities);
        context.SaveChanges();

        return entities;
    }

    private static TestEntity NewEntity(string name, EntityState state = EntityState.Saved, DateTimeOffset? modificationDate = null) {
        return new TestEntity {
            ID = Guid.NewGuid(),
            Name = name,
            EntityState = state,
            ModificationDate = modificationDate
        };
    }

    private TestEntity Load(Guid id) {
        using var context = CreateContext();

        return context.TestEntities.AsNoTracking().Single(entity => entity.ID == id);
    }

    // ---------------------------------------------------------------------------
    // ExecuteSoftDelete
    // ---------------------------------------------------------------------------

    [Fact]
    public void ExecuteSoftDelete_WithMatchingRows_SoftDeletesOnlyMatchingRows() {
        // arrange
        var entities = Seed(NewEntity("Alpha"), NewEntity("Beta"), NewEntity("Gamma"));

        using var context = CreateContext();

        // act
        var actual = context.TestEntities
                            .Where(entity => entity.Name != "Gamma")
                            .ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(2, actual),
            () => Assert.Equal(EntityState.Deleted, Load(entities[0].ID).EntityState),
            () => Assert.Equal(Now, Load(entities[0].ID).ModificationDate),
            () => Assert.Equal(EntityState.Deleted, Load(entities[1].ID).EntityState),
            () => Assert.Equal(Now, Load(entities[1].ID).ModificationDate),
            () => Assert.Equal(EntityState.Saved, Load(entities[2].ID).EntityState),
            () => Assert.Null(Load(entities[2].ID).ModificationDate)
        );
    }

    [Fact]
    public void ExecuteSoftDelete_WithAlreadySoftDeletedRows_SkipsThem() {
        // arrange
        var entities = Seed(
            NewEntity("Alpha"),
            NewEntity("Beta", EntityState.Deleted, Earlier)
        );

        using var context = CreateContext();

        // act
        var actual = context.TestEntities.ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, actual),
            () => Assert.Equal(Now, Load(entities[0].ID).ModificationDate),
            () => Assert.Equal(Earlier, Load(entities[1].ID).ModificationDate)
        );
    }

    [Fact]
    public void ExecuteSoftDelete_WithDirtyRows_SoftDeletesThem() {
        // arrange
        var entity = Seed(NewEntity("Alpha", EntityState.Dirty))[0];

        using var context = CreateContext();

        // act
        var actual = context.TestEntities.ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, actual),
            () => Assert.Equal(EntityState.Deleted, Load(entity.ID).EntityState)
        );
    }

    [Fact]
    public void ExecuteSoftDelete_WithNoMatchingRows_ReturnsZero() {
        // arrange
        Seed(NewEntity("Alpha"));

        using var context = CreateContext();

        // act
        var actual = context.TestEntities
                            .Where(entity => entity.Name == "DoesNotExist")
                            .ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public void ExecuteSoftDelete_DoesNotRemoveRows() {
        // arrange
        Seed(NewEntity("Alpha"), NewEntity("Beta"));

        using var context = CreateContext();

        // act
        context.TestEntities.ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Equal(2, context.TestEntities.Count());
    }

    [Fact]
    public void ExecuteSoftDelete_DoesNotUpdateTrackedEntities() {
        // arrange
        var id = Seed(NewEntity("Alpha"))[0].ID;

        using var context = CreateContext();
        var tracked = context.TestEntities.Single(entity => entity.ID == id);

        // act
        context.TestEntities.ExecuteSoftDelete(_timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(EntityState.Saved, tracked.EntityState),
            () => Assert.Null(tracked.ModificationDate),
            () => Assert.Equal(EntityState.Deleted, Load(id).EntityState)
        );
    }

    [Fact]
    public void ExecuteSoftDelete_WithoutTimeProvider_UsesSystemTime() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        using var context = CreateContext();
        var before = DateTimeOffset.UtcNow;

        // act
        context.TestEntities.ExecuteSoftDelete();

        // assert
        var after = DateTimeOffset.UtcNow;
        var actual = Load(entity.ID).ModificationDate;

        Assert.Multiple(
            () => Assert.NotNull(actual),
            () => Assert.InRange(actual!.Value, before, after)
        );
    }

    // ---------------------------------------------------------------------------
    // ExecuteSoftDeleteAsync
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task ExecuteSoftDeleteAsync_WithMatchingRows_SoftDeletesOnlyMatchingRows() {
        // arrange
        var entities = Seed(NewEntity("Alpha"), NewEntity("Beta"), NewEntity("Gamma"));

        await using var context = CreateContext();

        // act
        var actual = await context.TestEntities
                                  .Where(entity => entity.Name == "Beta")
                                  .ExecuteSoftDeleteAsync(TestContext.Current.CancellationToken, _timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, actual),
            () => Assert.Equal(EntityState.Saved, Load(entities[0].ID).EntityState),
            () => Assert.Equal(EntityState.Deleted, Load(entities[1].ID).EntityState),
            () => Assert.Equal(Now, Load(entities[1].ID).ModificationDate),
            () => Assert.Equal(EntityState.Saved, Load(entities[2].ID).EntityState)
        );
    }

    [Fact]
    public async Task ExecuteSoftDeleteAsync_WithAlreadySoftDeletedRows_SkipsThem() {
        // arrange
        var entities = Seed(
            NewEntity("Alpha"),
            NewEntity("Beta", EntityState.Deleted, Earlier)
        );

        await using var context = CreateContext();

        // act
        var actual = await context.TestEntities.ExecuteSoftDeleteAsync(TestContext.Current.CancellationToken, _timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(1, actual),
            () => Assert.Equal(Now, Load(entities[0].ID).ModificationDate),
            () => Assert.Equal(Earlier, Load(entities[1].ID).ModificationDate)
        );
    }

    [Fact]
    public async Task ExecuteSoftDeleteAsync_WithNoMatchingRows_ReturnsZero() {
        // arrange
        Seed(NewEntity("Alpha", EntityState.Deleted, Earlier));

        await using var context = CreateContext();

        // act
        var actual = await context.TestEntities.ExecuteSoftDeleteAsync(TestContext.Current.CancellationToken, _timeProvider);

        // assert
        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task ExecuteSoftDeleteAsync_WithoutTimeProvider_UsesSystemTime() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        await using var context = CreateContext();
        var before = DateTimeOffset.UtcNow;

        // act
        await context.TestEntities.ExecuteSoftDeleteAsync(TestContext.Current.CancellationToken);

        // assert
        var after = DateTimeOffset.UtcNow;
        var actual = Load(entity.ID).ModificationDate;

        Assert.Multiple(
            () => Assert.NotNull(actual),
            () => Assert.InRange(actual!.Value, before, after)
        );
    }

    [Fact]
    public async Task ExecuteSoftDeleteAsync_WithCanceledToken_ThrowsOperationCanceledException() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        await using var context = CreateContext();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // act & assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => context.TestEntities.ExecuteSoftDeleteAsync(cts.Token, _timeProvider)
        );

        Assert.Equal(EntityState.Saved, Load(entity.ID).EntityState);
    }
}
