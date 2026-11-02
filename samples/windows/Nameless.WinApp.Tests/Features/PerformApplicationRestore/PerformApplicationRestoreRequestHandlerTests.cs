using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Application;
using Nameless.IO.Physical;
using Nameless.Lucene;
using Nameless.Testing.Tools.IO;
using Nameless.WinApp.Data;
using Nameless.WinApp.Features.PerformApplicationBackup;
using Nameless.WinApp.Tests.Tooling;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Events;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.BackupRecovery;
using Nameless.Windows.Notification.SnackBar;

namespace Nameless.WinApp.Features.PerformApplicationRestore;

public class PerformApplicationRestoreRequestHandlerTests {
    private sealed class Sut : IDisposable {
        public required TempDirectory Root { get; init; }
        public required PhysicalFileManager FileManager { get; init; }
        public required Mock<IIndexProvider> IndexProviderMock { get; init; }
        public required Mock<IEventPublisher> EventPublisherMock { get; init; }
        public required AppDbContext DbContext { get; init; }
        public required PerformApplicationRestoreRequestHandler Handler { get; init; }

        public void Dispose() {
            DbContext.Dispose();
            Root.Dispose();
        }
    }

    private static Sut CreateSut(string luceneDirectoryRelativePath = "lucene") {
        var root = new TempDirectory(nameof(PerformApplicationRestoreRequestHandlerTests));
        var fileManager = new PhysicalFileManager(root.Path);

        var indexProviderMock = new Mock<IIndexProvider>();

        var eventPublisherMock = new Mock<IEventPublisher>();
        eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<BackupRecoveryProgressNotificationEvent>()))
            .Returns(Task.CompletedTask);
        eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<SnackBarNotificationEvent>()))
            .Returns(Task.CompletedTask);

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var dbContext = new AppDbContext(dbContextOptions);

        var luceneOptions = Options.Create(new LuceneOptions {
            DirectoryRelativePath = luceneDirectoryRelativePath
        });

        var handler = new PerformApplicationRestoreRequestHandler(
            fileManager: fileManager,
            dbContext: dbContext,
            indexProvider: indexProviderMock.Object,
            luceneOptions: luceneOptions,
            eventPublisher: eventPublisherMock.Object,
            translator: new FakeTranslator()
        );

        return new Sut {
            Root = root,
            FileManager = fileManager,
            IndexProviderMock = indexProviderMock,
            EventPublisherMock = eventPublisherMock,
            DbContext = dbContext,
            Handler = handler
        };
    }

    private static void CreateSqliteDatabase(string path, string value) {
        SysDirectory.CreateDirectory(SysPath.GetDirectoryName(path)!);

        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
            DataSource = path,
            Pooling = false
        }.ToString());

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE sample (value TEXT NOT NULL);";
        command.ExecuteNonQuery();

        command.CommandText = "INSERT INTO sample (value) VALUES ($value);";
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    private static string ReadSqliteValue(string path) {
        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
            DataSource = path,
            Pooling = false
        }.ToString());

        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM sample LIMIT 1;";

        return (string)command.ExecuteScalar()!;
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenRequestIsNull_ThrowsArgumentNullException() {
        // arrange
        using var sut = CreateSut();

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.Handler.HandleAsync(null!, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenArchiveIsCorrupt_RenamesOriginalArtifactsBackAndReturnsRolledBack() {
        // arrange
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, "<configuration>original</configuration>");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "original-value");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        var luceneFilePath = SysPath.Combine(luceneDirectoryPath, "segments.gen");
        SysFile.WriteAllText(luceneFilePath, "original-segment");

        // Not a valid zip at all: ZipFile.ExtractToDirectoryAsync throws
        // InvalidDataException while reading its central directory.
        var corruptZipPath = SysPath.Combine(sut.Root.Path, "corrupt.zip");
        SysFile.WriteAllText(corruptZipPath, "this is not a zip file");

        var request = new PerformApplicationRestoreRequest(corruptZipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);

        Assert.Equal("<configuration>original</configuration>", SysFile.ReadAllText(configPath));
        Assert.Equal("original-value", ReadSqliteValue(databasePath));
        Assert.Equal("original-segment", SysFile.ReadAllText(luceneFilePath));

        // No rollback siblings left lying around next to the restored originals.
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.IndexProviderMock.Verify(x => x.CloseAll(), Times.Once);

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Once
        );
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_RoundTrip_RestoresConfigDatabaseAndLuceneIndexFromBackup() {
        // arrange: back up a real small SQLite database and a fake index
        // folder using the already-shipped backup handler, then restore
        // from that archive and verify every file round-trips.
        using var sut = CreateSut();

        SysFile.WriteAllText(SysPath.Combine(sut.Root.Path, "app.config"), "<configuration>round-trip</configuration>");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "round-trip-value");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        SysFile.WriteAllText(SysPath.Combine(luceneDirectoryPath, "segments.gen"), "round-trip-segment");

        var backupHandler = new PerformApplicationBackupRequestHandler(
            applicationContext: CreateApplicationContextMock(sut.FileManager).Object,
            appConfigurationManager: CreateAppConfigurationManagerMock().Object,
            dbContext: sut.DbContext,
            indexProvider: sut.IndexProviderMock.Object,
            luceneOptions: Options.Create(new LuceneOptions { DirectoryRelativePath = "lucene" }),
            eventPublisher: sut.EventPublisherMock.Object,
            translator: new FakeTranslator()
        );

        var backupFilePath = await backupHandler.HandleAsync(new PerformApplicationBackupRequest(), TestContext.Current.CancellationToken);

        // Simulate loss/corruption of the live artifacts the backup captured.
        SysFile.Delete(SysPath.Combine(sut.Root.Path, "app.config"));
        SysFile.Delete(databasePath);
        SysDirectory.Delete(luceneDirectoryPath, recursive: true);

        var request = new PerformApplicationRestoreRequest(backupFilePath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.Completed, outcome);

        Assert.Equal("<configuration>round-trip</configuration>", SysFile.ReadAllText(SysPath.Combine(sut.Root.Path, "app.config")));
        Assert.Equal("round-trip-value", ReadSqliteValue(databasePath));
        Assert.Equal("round-trip-segment", SysFile.ReadAllText(SysPath.Combine(luceneDirectoryPath, "segments.gen")));

        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));
    }

    private static Mock<IApplicationContext> CreateApplicationContextMock(PhysicalFileManager fileManager) {
        var mock = new Mock<IApplicationContext>();
        mock.SetupGet(x => x.FileManager).Returns(fileManager);

        return mock;
    }

    private static Mock<IAppConfigurationManager> CreateAppConfigurationManagerMock() {
        var mock = new Mock<IAppConfigurationManager>();
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        return mock;
    }
}
