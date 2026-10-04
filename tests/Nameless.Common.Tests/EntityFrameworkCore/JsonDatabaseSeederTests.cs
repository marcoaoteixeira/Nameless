using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Nameless.EntityFrameworkCore.Entities;
using Nameless.IO;
using Nameless.Testing.Tools.Mockers.Logging;
using EmbeddedFileProvider = Nameless.IO.Embedded.FileProvider;
using PhysicalFileProvider = Nameless.IO.Physical.FileProvider;

namespace Nameless.EntityFrameworkCore;

public class JsonDatabaseSeederTests : IDisposable {
    private const string EMBEDDED_SEED_PATH = "EntityFrameworkCore/Resources/EmbeddedSeed.json";
    private const string EMBEDDED_MISSING_PATH = "EntityFrameworkCore/Resources/DoesNotExist.json";

    private readonly string _root;

    public JsonDatabaseSeederTests() {
        _root = SysPath.Combine(SysPath.GetTempPath(), $"nameless-test-{Guid.NewGuid():N}");
        SysDirectory.CreateDirectory(_root);
    }

    public void Dispose() {
        if (SysDirectory.Exists(_root)) {
            SysDirectory.Delete(_root, recursive: true);
        }
    }

    // ---------------------------------------------------------------------------
    // Inline test infrastructure
    // ---------------------------------------------------------------------------

    public sealed class TestEntity : EntityBase {
        public string Name { get; set; } = string.Empty;
    }

    public sealed class RecordingSeeder : JsonDatabaseSeeder<TestEntity> {
        public TestEntity[]? CapturedAsyncSeeds { get; private set; }
        public TestEntity[]? CapturedSyncSeeds { get; private set; }

        public override JsonDatabaseSeederOptions Options { get; }

        public override int Order => 0;

        public RecordingSeeder(IFileProvider fileProvider, JsonDatabaseSeederOptions options)
            : base(fileProvider, new LoggerMocker<RecordingSeeder>().WithAnyLogLevel().Build()) {
            Options = options;
        }

        protected override Task ExecuteAsyncCore(DbContext dbContext, TestEntity[] seeds, bool storeManagementOperation, CancellationToken cancellationToken) {
            CapturedAsyncSeeds = seeds;
            return Task.CompletedTask;
        }

        protected override void ExecuteCore(DbContext dbContext, TestEntity[] seeds, bool storeManagementOperation) {
            CapturedSyncSeeds = seeds;
        }
    }

    private sealed class TestDbContext : DbContext {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
    }

    // ---------------------------------------------------------------------------
    // Factory helpers
    // ---------------------------------------------------------------------------

    private static TestDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        return new TestDbContext(options);
    }

    private static EmbeddedFileProvider CreateEmbeddedFileProvider() {
        return new EmbeddedFileProvider(typeof(JsonDatabaseSeederTests).Assembly);
    }

    private PhysicalFileProvider CreatePhysicalFileProvider() {
        return new PhysicalFileProvider(_root);
    }

    private void WritePhysicalFile(string relativePath, string content) {
        var path = SysPath.Combine(_root, relativePath);

        SysDirectory.CreateDirectory(SysPath.GetDirectoryName(path) ?? _root);
        SysFile.WriteAllText(path, content);
    }

    // ---------------------------------------------------------------------------
    // Embedded file provider
    // ---------------------------------------------------------------------------

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_WithEmbeddedFileProvider_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = EMBEDDED_SEED_PATH };
        var sut = new RecordingSeeder(CreateEmbeddedFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Single(sut.CapturedAsyncSeeds!),
            () => Assert.Equal("EmbeddedAlpha", sut.CapturedAsyncSeeds![0].Name)
        );
    }

    [Fact]
    [UnitTest]
    public void Execute_WithEmbeddedFileProvider_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = EMBEDDED_SEED_PATH };
        var sut = new RecordingSeeder(CreateEmbeddedFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedSyncSeeds),
            () => Assert.Single(sut.CapturedSyncSeeds!),
            () => Assert.Equal("EmbeddedAlpha", sut.CapturedSyncSeeds![0].Name)
        );
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_WithEmbeddedFileProvider_WhenFileMissing_AndThrowOnMissingFalse_PassesEmptyArray() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = EMBEDDED_MISSING_PATH, ThrowOnMissing = false };
        var sut = new RecordingSeeder(CreateEmbeddedFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Empty(sut.CapturedAsyncSeeds!)
        );
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_WithEmbeddedFileProvider_WhenFileMissing_AndThrowOnMissingTrue_ThrowsMissingDatabaseSeederResourceException() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = EMBEDDED_MISSING_PATH, ThrowOnMissing = true };
        var sut = new RecordingSeeder(CreateEmbeddedFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act & assert
        var exception = await Assert.ThrowsAsync<MissingDatabaseSeederResourceException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );

        Assert.Contains(EMBEDDED_MISSING_PATH, exception.Message);
    }

    [Fact]
    [UnitTest]
    public void Execute_WithEmbeddedFileProvider_WhenFileMissing_AndThrowOnMissingTrue_ThrowsMissingDatabaseSeederResourceException() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = EMBEDDED_MISSING_PATH, ThrowOnMissing = true };
        var sut = new RecordingSeeder(CreateEmbeddedFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act & assert
        Assert.Throws<MissingDatabaseSeederResourceException>(
            () => sut.Execute(dbContext, storeManagementOperation: false)
        );
    }

    // ---------------------------------------------------------------------------
    // Physical file provider
    // ---------------------------------------------------------------------------

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        WritePhysicalFile("seeds.json", """[{"Name":"Alpha"},{"Name":"Beta"}]""");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Equal(2, sut.CapturedAsyncSeeds!.Length),
            () => Assert.Equal("Alpha", sut.CapturedAsyncSeeds![0].Name),
            () => Assert.Equal("Beta", sut.CapturedAsyncSeeds![1].Name)
        );
    }

    [Fact]
    [IntegrationTest]
    public void Execute_WithPhysicalFileProvider_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        WritePhysicalFile("seeds.json", """[{"Name":"Gamma"}]""");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedSyncSeeds),
            () => Assert.Single(sut.CapturedSyncSeeds!),
            () => Assert.Equal("Gamma", sut.CapturedSyncSeeds![0].Name)
        );
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenFileInSubDirectory_DeserializesAndPassesSeeds() {
        // arrange
        WritePhysicalFile("Seeds/Nested/seeds.json", """[{"Name":"Nested"}]""");

        var options = new JsonDatabaseSeederOptions { RelativePath = "Seeds/Nested/seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal("Nested", Assert.Single(sut.CapturedAsyncSeeds!).Name);
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenEnumAsString_DeserializesEnum() {
        // arrange
        WritePhysicalFile("seeds.json", """[{"Name":"Draft","EntityState":"Dirty"}]""");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(Entities.EntityState.Dirty, Assert.Single(sut.CapturedAsyncSeeds!).EntityState);
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenFileMissing_AndThrowOnMissingFalse_PassesEmptyArray() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = "missing.json", ThrowOnMissing = false };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Empty(sut.CapturedAsyncSeeds!)
        );
    }

    [Fact]
    [IntegrationTest]
    public void Execute_WithPhysicalFileProvider_WhenFileMissing_AndThrowOnMissingFalse_PassesEmptyArray() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = "missing.json", ThrowOnMissing = false };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedSyncSeeds),
            () => Assert.Empty(sut.CapturedSyncSeeds!)
        );
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenFileMissing_AndThrowOnMissingTrue_ThrowsMissingDatabaseSeederResourceException() {
        // arrange
        var options = new JsonDatabaseSeederOptions { RelativePath = "missing.json", ThrowOnMissing = true };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act & assert
        var exception = await Assert.ThrowsAsync<MissingDatabaseSeederResourceException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );

        Assert.Contains("missing.json", exception.Message);
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenJsonIsNull_ThrowsInvalidOperationException() {
        // arrange
        WritePhysicalFile("seeds.json", "null");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [IntegrationTest]
    public void Execute_WithPhysicalFileProvider_WhenJsonIsNull_ThrowsInvalidOperationException() {
        // arrange
        WritePhysicalFile("seeds.json", "null");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Execute(dbContext, storeManagementOperation: false));
    }

    [Fact]
    [IntegrationTest]
    public async Task ExecuteAsync_WithPhysicalFileProvider_WhenJsonMalformed_ThrowsJsonException() {
        // arrange
        WritePhysicalFile("seeds.json", "[{ not json");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        await using var dbContext = CreateDbContext();

        // act & assert
        await Assert.ThrowsAnyAsync<JsonException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [IntegrationTest]
    public void Execute_WithPhysicalFileProvider_AfterExecution_ReleasesFileHandle() {
        // arrange
        WritePhysicalFile("seeds.json", """[{"Name":"Alpha"}]""");

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(CreatePhysicalFileProvider(), options);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert — would throw IOException on Windows if the stream was left open
        var exception = Record.Exception(() => SysFile.Delete(SysPath.Combine(_root, "seeds.json")));

        Assert.Null(exception);
    }
}
