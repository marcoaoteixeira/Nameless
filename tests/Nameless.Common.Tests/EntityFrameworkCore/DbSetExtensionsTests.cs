using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Nameless.EntityFrameworkCore.Entities;
using EfEntityState = Microsoft.EntityFrameworkCore.EntityState;
using EntityState = Nameless.EntityFrameworkCore.Entities.EntityState;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class DbSetExtensionsTests : IDisposable {
    private static readonly DateTimeOffset Now = new(2026, 3, 10, 8, 30, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Earlier = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private readonly SqliteConnection _connection;
    private readonly FakeTimeProvider _timeProvider = new(Now);

    public DbSetExtensionsTests() {
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

    private TestEntity? Load(Guid id) {
        using var context = CreateContext();

        return context.TestEntities.AsNoTracking().SingleOrDefault(entity => entity.ID == id);
    }

    // ---------------------------------------------------------------------------
    // SoftRemove
    // ---------------------------------------------------------------------------

    [Fact]
    public void SoftRemove_WithDetachedPersistedEntity_MarksEntityAsDeleted() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        using var context = CreateContext();

        // act
        context.TestEntities.SoftRemove(entity, _timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(EntityState.Deleted, entity.EntityState),
            () => Assert.Equal(Now, entity.ModificationDate)
        );
    }

    [Fact]
    public void SoftRemove_WithDetachedPersistedEntity_ReturnsModifiedEntryWithOnlySoftDeletePropertiesModified() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        using var context = CreateContext();

        // act
        var entry = context.TestEntities.SoftRemove(entity, _timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Same(entity, entry.Entity),
            () => Assert.Equal(EfEntityState.Modified, entry.State),
            () => Assert.True(entry.Property(x => x.EntityState).IsModified),
            () => Assert.True(entry.Property(x => x.ModificationDate).IsModified),
            () => Assert.False(entry.Property(x => x.Name).IsModified),
            () => Assert.False(entry.Property(x => x.CreationDate).IsModified)
        );
    }

    [Fact]
    public void SoftRemove_WithDetachedPersistedEntity_WhenSaved_PersistsSoftDeleteOnly() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        // local change that must NOT be persisted, since only the
        // soft delete properties are flagged as modified.
        entity.Name = "Changed";

        using (var context = CreateContext()) {
            // act
            context.TestEntities.SoftRemove(entity, _timeProvider);
            context.SaveChanges();
        }

        // assert
        var actual = Load(entity.ID);

        Assert.Multiple(
            () => Assert.NotNull(actual),
            () => Assert.Equal(EntityState.Deleted, actual!.EntityState),
            () => Assert.Equal(Now, actual!.ModificationDate),
            () => Assert.Equal("Alpha", actual!.Name)
        );
    }

    [Fact]
    public void SoftRemove_WithTrackedUnchangedEntity_MarksEntryAsModified() {
        // arrange
        var id = Seed(NewEntity("Alpha"))[0].ID;

        using var context = CreateContext();
        var entity = context.TestEntities.Single(x => x.ID == id);

        // act
        var entry = context.TestEntities.SoftRemove(entity, _timeProvider);
        context.SaveChanges();

        // assert
        var actual = Load(id);

        Assert.Multiple(
            () => Assert.Equal(EfEntityState.Unchanged, entry.State), // after SaveChanges
            () => Assert.Equal(EntityState.Deleted, actual!.EntityState),
            () => Assert.Equal(Now, actual!.ModificationDate)
        );
    }

    [Fact]
    public void SoftRemove_WithTrackedModifiedEntity_KeepsPendingChanges() {
        // arrange
        var id = Seed(NewEntity("Alpha"))[0].ID;

        using var context = CreateContext();
        var entity = context.TestEntities.Single(x => x.ID == id);
        entity.Name = "Changed";

        // act
        context.TestEntities.SoftRemove(entity, _timeProvider);
        context.SaveChanges();

        // assert
        var actual = Load(id);

        Assert.Multiple(
            () => Assert.Equal(EntityState.Deleted, actual!.EntityState),
            () => Assert.Equal("Changed", actual!.Name)
        );
    }

    [Fact]
    public void SoftRemove_WithAlreadySoftDeletedEntity_DoesNotChangeEntity() {
        // arrange
        var entity = Seed(NewEntity("Alpha", EntityState.Deleted, Earlier))[0];

        using var context = CreateContext();

        // act
        var entry = context.TestEntities.SoftRemove(entity, _timeProvider);

        // assert
        Assert.Multiple(
            () => Assert.Equal(EfEntityState.Unchanged, entry.State),
            () => Assert.Equal(EntityState.Deleted, entity.EntityState),
            () => Assert.Equal(Earlier, entity.ModificationDate),
            () => Assert.False(entry.Property(x => x.ModificationDate).IsModified)
        );
    }

    [Fact]
    public void SoftRemove_WithAddedEntity_CancelsPendingInsert() {
        // arrange
        using var context = CreateContext();

        var entity = NewEntity("Alpha");
        context.TestEntities.Add(entity);

        // act
        var entry = context.TestEntities.SoftRemove(entity, _timeProvider);
        context.SaveChanges();

        // assert
        Assert.Multiple(
            () => Assert.Equal(EfEntityState.Detached, entry.State),
            () => Assert.Null(Load(entity.ID))
        );
    }

    [Fact]
    public void SoftRemove_WithNewEntityWithoutKey_DoesNotInsert() {
        // arrange — Attach marks entities with unset generated keys as Added.
        using var context = CreateContext();

        var entity = new TestEntity { Name = "Alpha" };

        // act
        var entry = context.TestEntities.SoftRemove(entity, _timeProvider);
        context.SaveChanges();

        // assert
        using var verification = CreateContext();

        Assert.Multiple(
            () => Assert.Equal(EfEntityState.Detached, entry.State),
            () => Assert.Empty(verification.TestEntities.AsNoTracking())
        );
    }

    [Fact]
    public void SoftRemove_WithoutTimeProvider_UsesSystemTime() {
        // arrange
        var entity = Seed(NewEntity("Alpha"))[0];

        using var context = CreateContext();
        var before = DateTimeOffset.UtcNow;

        // act
        context.TestEntities.SoftRemove(entity);

        // assert
        var after = DateTimeOffset.UtcNow;

        Assert.Multiple(
            () => Assert.NotNull(entity.ModificationDate),
            () => Assert.InRange(entity.ModificationDate!.Value, before, after),
            () => Assert.Equal(TimeSpan.Zero, entity.ModificationDate!.Value.Offset)
        );
    }

    // ---------------------------------------------------------------------------
    // SoftRemoveRange
    // ---------------------------------------------------------------------------

    [Fact]
    public void SoftRemoveRange_WithMultipleEntities_MarksAllAsDeletedWithSameTimestamp() {
        // arrange
        var entities = Seed(NewEntity("Alpha"), NewEntity("Beta"), NewEntity("Gamma"));

        using (var context = CreateContext()) {
            // act
            context.TestEntities.SoftRemoveRange(entities, _timeProvider);
            context.SaveChanges();
        }

        // assert
        var actual = entities.Select(entity => Load(entity.ID)).ToArray();

        Assert.All(actual, entity => {
            Assert.NotNull(entity);
            Assert.Equal(EntityState.Deleted, entity.EntityState);
            Assert.Equal(Now, entity.ModificationDate);
        });
    }

    [Fact]
    public void SoftRemoveRange_WithAlreadySoftDeletedEntity_SkipsIt() {
        // arrange
        var entities = Seed(
            NewEntity("Alpha"),
            NewEntity("Beta", EntityState.Deleted, Earlier)
        );

        using (var context = CreateContext()) {
            // act
            context.TestEntities.SoftRemoveRange(entities, _timeProvider);
            context.SaveChanges();
        }

        // assert
        Assert.Multiple(
            () => Assert.Equal(Now, Load(entities[0].ID)!.ModificationDate),
            () => Assert.Equal(EntityState.Deleted, Load(entities[1].ID)!.EntityState),
            () => Assert.Equal(Earlier, Load(entities[1].ID)!.ModificationDate)
        );
    }

    [Fact]
    public void SoftRemoveRange_WithEmptyArray_DoesNothing() {
        // arrange
        using var context = CreateContext();

        // act
        context.TestEntities.SoftRemoveRange([], _timeProvider);

        // assert
        Assert.False(context.ChangeTracker.HasChanges());
    }

    [Fact]
    public void SoftRemoveRange_WithMixOfAddedAndPersistedEntities_CancelsInsertAndSoftDeletesPersisted() {
        // arrange
        var persisted = Seed(NewEntity("Alpha"))[0];

        using var context = CreateContext();

        var added = NewEntity("Beta");
        context.TestEntities.Add(added);

        // act
        context.TestEntities.SoftRemoveRange([persisted, added], _timeProvider);
        context.SaveChanges();

        // assert
        Assert.Multiple(
            () => Assert.Equal(EntityState.Deleted, Load(persisted.ID)!.EntityState),
            () => Assert.Null(Load(added.ID))
        );
    }
}
