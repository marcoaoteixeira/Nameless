using Microsoft.EntityFrameworkCore;
using Moq;
using Nameless.EntityFrameworkCore;
using Nameless.EntityFrameworkCore.Entities;
using Nameless.Testing.Tools.Attributes;
using Xunit;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class AuditableSaveChangesInterceptorTests {
    // ---------------------------------------------------------------------------
    // Inline test infrastructure
    // ---------------------------------------------------------------------------

    private sealed class TestEntity : EntityBase<Guid> {
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

    private static TestDbContext CreateContext(TimeProvider timeProvider) {
        var interceptor = new AuditableSaveChangesInterceptor(timeProvider);

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Data Source=:memory:")
            .AddInterceptors(interceptor)
            .Options;

        var context = new TestDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    // ---------------------------------------------------------------------------
    // Tests
    // ---------------------------------------------------------------------------

    [Fact]
    public async Task SavingChangesAsync_NewEntity_SetsCreationDate() {
        // arrange
        var expectedDate = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(tp => tp.GetUtcNow()).Returns(expectedDate);

        await using var context = CreateContext(timeProviderMock.Object);

        var entity = new TestEntity { ID = Guid.NewGuid(), Name = "Alpha" };
        context.TestEntities.Add(entity);

        // act
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(expectedDate, entity.CreationDate);
    }

    [Fact]
    public async Task SavingChangesAsync_NewEntity_LeavesModificationDateNull() {
        // arrange
        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(tp => tp.GetUtcNow())
                        .Returns(new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero));

        await using var context = CreateContext(timeProviderMock.Object);

        var entity = new TestEntity { ID = Guid.NewGuid(), Name = "Beta" };
        context.TestEntities.Add(entity);

        // act
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Null(entity.ModificationDate);
    }

    [Fact]
    public async Task SavingChangesAsync_ModifiedEntity_SetsModificationDate() {
        // arrange
        var creationDate = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var modificationDate = new DateTimeOffset(2026, 2, 20, 14, 30, 0, TimeSpan.Zero);

        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.SetupSequence(tp => tp.GetUtcNow())
                        .Returns(creationDate)
                        .Returns(modificationDate);

        await using var context = CreateContext(timeProviderMock.Object);

        var entity = new TestEntity { ID = Guid.NewGuid(), Name = "Gamma" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act — mark as modified and save again
        entity.Name = "Gamma Updated";
        context.TestEntities.Update(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(modificationDate, entity.ModificationDate);
    }

    [Fact]
    public async Task SavingChangesAsync_ModifiedEntity_PreservesCreationDate() {
        // arrange
        var creationDate = new DateTimeOffset(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
        var modificationDate = new DateTimeOffset(2026, 2, 20, 14, 30, 0, TimeSpan.Zero);

        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.SetupSequence(tp => tp.GetUtcNow())
                        .Returns(creationDate)
                        .Returns(modificationDate);

        await using var context = CreateContext(timeProviderMock.Object);

        var entity = new TestEntity { ID = Guid.NewGuid(), Name = "Delta" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        entity.Name = "Delta Updated";
        context.TestEntities.Update(entity);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(creationDate, entity.CreationDate);
    }
}
