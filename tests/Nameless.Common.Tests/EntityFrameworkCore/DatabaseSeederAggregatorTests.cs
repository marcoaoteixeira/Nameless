using Microsoft.EntityFrameworkCore;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.EntityFrameworkCore;

[IntegrationTest]
public class DatabaseSeederAggregatorTests {
    private sealed class RecordingDbContext : DbContext {
        public RecordingDbContext(DbContextOptions<RecordingDbContext> options) : base(options) { }
    }

    private sealed class RecordingSeeder : IDatabaseSeeder {
        private readonly List<string> _log;

        public int Order { get; }

        public RecordingSeeder(int order, List<string> log) {
            Order = order;
            _log = log;
        }

        public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken) {
            _log.Add($"async:{Order}");
            return Task.CompletedTask;
        }

        public void Execute(DbContext dbContext, bool storeManagementOperation) {
            _log.Add($"sync:{Order}");
        }
    }

    private sealed class ThrowingSeeder : IDatabaseSeeder {
        public int Order => 0;

        public Task ExecuteAsync(DbContext dbContext, bool storeManagementOperation, CancellationToken cancellationToken)
            => throw new InvalidOperationException("boom");

        public void Execute(DbContext dbContext, bool storeManagementOperation)
            => throw new InvalidOperationException("boom");
    }

    private static RecordingDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<RecordingDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        var context = new RecordingDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    [Fact]
    public async Task ExecuteAsync_RunsSeedersInOrder() {
        // arrange
        var log = new List<string>();
        var seeders = new IDatabaseSeeder[] {
            new RecordingSeeder(2, log),
            new RecordingSeeder(1, log)
        };

        var logger = new LoggerMocker<DatabaseSeederAggregator>().WithAnyLogLevel().Build();
        var sut = new DatabaseSeederAggregator(seeders, logger);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["async:1", "async:2"], log);
    }

    [Fact]
    public void Execute_RunsSeedersInOrder() {
        // arrange
        var log = new List<string>();
        var seeders = new IDatabaseSeeder[] {
            new RecordingSeeder(2, log),
            new RecordingSeeder(1, log)
        };

        var logger = new LoggerMocker<DatabaseSeederAggregator>().WithAnyLogLevel().Build();
        var sut = new DatabaseSeederAggregator(seeders, logger);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert
        Assert.Equal(["sync:1", "sync:2"], log);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSeederThrows_PropagatesException() {
        // arrange
        var seeders = new IDatabaseSeeder[] { new ThrowingSeeder() };
        var logger = new LoggerMocker<DatabaseSeederAggregator>().WithAnyLogLevel().Build();
        var sut = new DatabaseSeederAggregator(seeders, logger);

        await using var dbContext = CreateDbContext();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public void Execute_WhenSeederThrows_PropagatesException() {
        // arrange
        var seeders = new IDatabaseSeeder[] { new ThrowingSeeder() };
        var logger = new LoggerMocker<DatabaseSeederAggregator>().WithAnyLogLevel().Build();
        var sut = new DatabaseSeederAggregator(seeders, logger);

        using var dbContext = CreateDbContext();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Execute(dbContext, storeManagementOperation: false));
    }

    [Fact]
    public void Order_IsMinValue() {
        // arrange
        var logger = new LoggerMocker<DatabaseSeederAggregator>().WithAnyLogLevel().Build();
        var sut = new DatabaseSeederAggregator([], logger);

        // act & assert
        Assert.Equal(int.MinValue, sut.Order);
    }
}
