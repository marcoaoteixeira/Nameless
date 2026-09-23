using Microsoft.EntityFrameworkCore;
using Nameless.EntityFrameworkCore.Entities;
using Nameless.IO;
using Nameless.Testing.Tools.Mockers.Logging;
using Moq;

namespace Nameless.EntityFrameworkCore;

[UnitTest]
public class JsonDatabaseSeederTests {
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

    private static TestDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        return new TestDbContext(options);
    }

    private static Mock<IFile> CreateFileMock(bool exists, string? content = null) {
        var fileMock = new Mock<IFile>();
        fileMock.Setup(f => f.Exists).Returns(exists);

        if (exists && content is not null) {
            fileMock.Setup(f => f.Open(It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
                    .Returns(() => new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)));
        }

        return fileMock;
    }

    [Fact]
    public async Task ExecuteAsync_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        const string Json = """[{"Name":"Alpha"},{"Name":"Beta"}]""";
        var fileMock = CreateFileMock(exists: true, content: Json);

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("seeds.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Equal(2, sut.CapturedAsyncSeeds!.Length),
            () => Assert.Equal("Alpha", sut.CapturedAsyncSeeds![0].Name)
        );
    }

    [Fact]
    public void Execute_WhenFileExists_DeserializesAndPassesSeeds() {
        // arrange
        const string Json = """[{"Name":"Gamma"}]""";
        var fileMock = CreateFileMock(exists: true, content: Json);

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("seeds.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(providerMock.Object, options);

        using var dbContext = CreateDbContext();

        // act
        sut.Execute(dbContext, storeManagementOperation: false);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedSyncSeeds),
            () => Assert.Single(sut.CapturedSyncSeeds!)
        );
    }

    [Fact]
    public async Task ExecuteAsync_WhenFileMissing_AndThrowOnMissingFalse_PassesEmptyArray() {
        // arrange
        var fileMock = CreateFileMock(exists: false);

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("missing.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "missing.json", ThrowOnMissing = false };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Empty(sut.CapturedAsyncSeeds!);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFileMissing_AndThrowOnMissingTrue_ThrowsMissingDatabaseSeederResourceException() {
        // arrange
        var fileMock = CreateFileMock(exists: false);

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("missing.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "missing.json", ThrowOnMissing = true };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act & assert
        await Assert.ThrowsAsync<MissingDatabaseSeederResourceException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithEmbeddedResource_DeserializesAndPassesSeeds() {
        // arrange
        var providerMock = new Mock<IFileProvider>();

        var options = new JsonDatabaseSeederOptions {
            RelativePath = "Nameless/EntityFrameworkCore/Resources/EmbeddedSeed.json",
            UseEmbeddedResource = true,
            Assembly = typeof(JsonDatabaseSeederTests).Assembly
        };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(sut.CapturedAsyncSeeds),
            () => Assert.Equal("EmbeddedAlpha", sut.CapturedAsyncSeeds![0].Name)
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithEmbeddedResource_WhenMissing_AndThrowOnMissingFalse_PassesEmptyArray() {
        // arrange
        var providerMock = new Mock<IFileProvider>();

        var options = new JsonDatabaseSeederOptions {
            RelativePath = "Nameless/EntityFrameworkCore/Resources/DoesNotExist.json",
            UseEmbeddedResource = true,
            Assembly = typeof(JsonDatabaseSeederTests).Assembly,
            ThrowOnMissing = false
        };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act
        await sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken);

        // assert
        Assert.Empty(sut.CapturedAsyncSeeds!);
    }

    [Fact]
    public void Execute_WhenSeederThrows_PropagatesException() {
        // arrange
        var fileMock = CreateFileMock(exists: true, content: "null");

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("seeds.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(providerMock.Object, options);

        using var dbContext = CreateDbContext();

        // act & assert
        Assert.Throws<InvalidOperationException>(() => sut.Execute(dbContext, storeManagementOperation: false));
    }

    [Fact]
    public async Task ExecuteAsync_WhenJsonInvalid_ThrowsInvalidOperationException() {
        // arrange
        var fileMock = CreateFileMock(exists: true, content: "null");

        var providerMock = new Mock<IFileProvider>();
        providerMock.Setup(p => p.GetFile("seeds.json")).Returns(fileMock.Object);

        var options = new JsonDatabaseSeederOptions { RelativePath = "seeds.json" };
        var sut = new RecordingSeeder(providerMock.Object, options);

        await using var dbContext = CreateDbContext();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(dbContext, storeManagementOperation: false, TestContext.Current.CancellationToken)
        );
    }
}
