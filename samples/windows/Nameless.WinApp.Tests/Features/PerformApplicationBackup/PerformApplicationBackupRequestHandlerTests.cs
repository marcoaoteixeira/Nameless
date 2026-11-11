using System.IO;
using System.IO.Compression;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Application;
using Nameless.IO.Physical;
using Nameless.Lucene;
using Nameless.Testing.Tools.IO;
using Nameless.WinApp.Data;
using Nameless.WinApp.Tests.Tooling;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Events;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.BackupRecovery;

namespace Nameless.WinApp.Features.PerformApplicationBackup;

public class PerformApplicationBackupRequestHandlerTests {
    private sealed class Sut : IDisposable {
        public required TempDirectory Root { get; init; }
        public required PhysicalFileManager FileManager { get; init; }
        public required Mock<IAppConfigurationManager> AppConfigurationManagerMock { get; init; }
        public required Mock<IIndexProvider> IndexProviderMock { get; init; }
        public required Mock<IEventPublisher> EventPublisherMock { get; init; }
        public required AppDbContext DbContext { get; init; }
        public required PerformApplicationBackupRequestHandler Handler { get; init; }

        public void Dispose() {
            DbContext.Dispose();
            Root.Dispose();
        }
    }

    private static Sut CreateSut(string? luceneDirectoryRelativePath = "lucene") {
        var root = new TempDirectory(nameof(PerformApplicationBackupRequestHandlerTests));
        var fileManager = new PhysicalFileManager(root.Path);

        var applicationContextMock = new Mock<IApplicationContext>();
        applicationContextMock.SetupGet(x => x.FileManager).Returns(fileManager);

        var appConfigurationManagerMock = new Mock<IAppConfigurationManager>();
        appConfigurationManagerMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var indexProviderMock = new Mock<IIndexProvider>();

        var eventPublisherMock = new Mock<IEventPublisher>();
        eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<BackupRecoveryProgressNotificationEvent>()))
            .Returns(Task.CompletedTask);

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var dbContext = new AppDbContext(dbContextOptions);

        var luceneOptions = Options.Create(new LuceneOptions {
            DirectoryRelativePath = luceneDirectoryRelativePath ?? "lucene"
        });

        var handler = new PerformApplicationBackupRequestHandler(
            applicationContext: applicationContextMock.Object,
            appConfigurationManager: appConfigurationManagerMock.Object,
            dbContext: dbContext,
            indexProvider: indexProviderMock.Object,
            luceneOptions: luceneOptions,
            eventPublisher: eventPublisherMock.Object,
            translator: FakeTranslator.For<PerformApplicationBackupRequestHandler>()
        );

        return new Sut {
            Root = root,
            FileManager = fileManager,
            AppConfigurationManagerMock = appConfigurationManagerMock,
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
    [IntegrationTest]
    public async Task HandleAsync_WhenAllArtifactsExist_CreatesArchiveContainingConfigDatabaseAndLuceneIndex() {
        // arrange
        using var sut = CreateSut();

        SysFile.WriteAllText(SysPath.Combine(sut.Root.Path, "app.config"), "<configuration />");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "hello-backup");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        SysFile.WriteAllText(SysPath.Combine(luceneDirectoryPath, "segments.gen"), "segment-data");
        SysFile.WriteAllText(SysPath.Combine(luceneDirectoryPath, "write.lock"), string.Empty);

        var request = new PerformApplicationBackupRequest();

        // act
        var archivePath = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.True(SysFile.Exists(archivePath));
        Assert.StartsWith(SysPath.Combine(sut.Root.Path, "backups"), archivePath);

        using var extracted = new TempDirectory(nameof(HandleAsync_WhenAllArtifactsExist_CreatesArchiveContainingConfigDatabaseAndLuceneIndex));
        ZipFile.ExtractToDirectory(archivePath, extracted.Path);

        Assert.True(SysFile.Exists(SysPath.Combine(extracted.Path, "app.config")));
        Assert.True(SysFile.Exists(SysPath.Combine(extracted.Path, "app.db")));
        Assert.True(SysFile.Exists(SysPath.Combine(extracted.Path, "lucene", "segments.gen")));
        Assert.False(SysFile.Exists(SysPath.Combine(extracted.Path, "lucene", "write.lock")));

        Assert.Equal("hello-backup", ReadSqliteValue(SysPath.Combine(extracted.Path, "app.db")));

        sut.IndexProviderMock.Verify(x => x.CloseAll(), Times.Once);
        sut.AppConfigurationManagerMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        var temporaryRoot = SysPath.Combine(sut.Root.Path, "temporary");
        Assert.True(!SysDirectory.Exists(temporaryRoot) || SysDirectory.GetDirectories(temporaryRoot).Length == 0);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenConfigurationFileIsMissing_StillCreatesArchiveAndPublishesWarning() {
        // arrange
        using var sut = CreateSut();
        var request = new PerformApplicationBackupRequest();

        // act
        var archivePath = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.True(SysFile.Exists(archivePath));

        using var extracted = new TempDirectory(nameof(HandleAsync_WhenConfigurationFileIsMissing_StillCreatesArchiveAndPublishesWarning));
        ZipFile.ExtractToDirectory(archivePath, extracted.Path);

        Assert.False(SysFile.Exists(SysPath.Combine(extracted.Path, "app.config")));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<BackupRecoveryProgressNotificationEvent>(e => e.Type == NotificationType.Warning)),
            Times.Once
        );
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenDatabaseFileIsMissing_SkipsDatabaseBackup() {
        // arrange
        using var sut = CreateSut();
        var request = new PerformApplicationBackupRequest();

        // act
        var archivePath = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        using var extracted = new TempDirectory(nameof(HandleAsync_WhenDatabaseFileIsMissing_SkipsDatabaseBackup));
        ZipFile.ExtractToDirectory(archivePath, extracted.Path);

        Assert.False(SysFile.Exists(SysPath.Combine(extracted.Path, "app.db")));
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenLuceneDirectoryIsMissing_SkipsLuceneBackup() {
        // arrange
        using var sut = CreateSut();
        var request = new PerformApplicationBackupRequest();

        // act
        var archivePath = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        using var extracted = new TempDirectory(nameof(HandleAsync_WhenLuceneDirectoryIsMissing_SkipsLuceneBackup));
        ZipFile.ExtractToDirectory(archivePath, extracted.Path);

        Assert.False(SysDirectory.Exists(SysPath.Combine(extracted.Path, "lucene")));
        sut.IndexProviderMock.Verify(x => x.CloseAll(), Times.Once);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_AlwaysClosesEveryIndexBeforeArchiving() {
        // arrange
        using var sut = CreateSut();
        var request = new PerformApplicationBackupRequest();

        // act
        await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        sut.IndexProviderMock.Verify(x => x.CloseAll(), Times.Once);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenSavingConfigurationFails_PropagatesExceptionAndPublishesFailure() {
        // arrange
        using var sut = CreateSut();
        var failure = new InvalidOperationException("boom");

        sut.AppConfigurationManagerMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        var request = new PerformApplicationBackupRequest();

        // act & assert
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken)
        );

        Assert.Same(failure, thrown);

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<BackupRecoveryProgressNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Once
        );

        var temporaryRoot = SysPath.Combine(sut.Root.Path, "temporary");
        Assert.True(!SysDirectory.Exists(temporaryRoot) || SysDirectory.GetDirectories(temporaryRoot).Length == 0);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenGetDestinationFilePathFails_CleansUpTemporaryDirectory() {
        // arrange
        using var sut = CreateSut();

        // Point the backup directory at a location that cannot be a
        // directory (a file occupying the name) so GetDestinationFilePath's
        // backupDirectory.Create() fails deterministically after every prior
        // step has succeeded. This happens before the archive try/catch, so
        // it is a different failure path from the one below.
        var backupsPath = SysPath.Combine(sut.Root.Path, "backups");
        SysFile.WriteAllText(backupsPath, "not-a-directory");

        var request = new PerformApplicationBackupRequest();

        // act & assert
        await Assert.ThrowsAnyAsync<Exception>(
            () => sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken)
        );

        var temporaryRoot = SysPath.Combine(sut.Root.Path, "temporary");
        Assert.True(!SysDirectory.Exists(temporaryRoot) || SysDirectory.GetDirectories(temporaryRoot).Length == 0);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenArchivingFails_DeletesPartialArchiveAndPublishesFailure() {
        // arrange
        using var sut = CreateSut();

        SysFile.WriteAllText(SysPath.Combine(sut.Root.Path, "app.config"), "<configuration />");

        var request = new PerformApplicationBackupRequest();
        FileStream? lockedCopy = null;

        // By the time the "CreatingArchive" progress event fires, the
        // per-run temp directory has already been populated; lock one of
        // its files so ZipFile.CreateFromDirectoryAsync fails while reading
        // it, exercising the real step-8 try/catch (DeletePartialArchive +
        // failure publish) instead of a step that runs before it.
        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<BackupRecoveryProgressNotificationEvent>(
                e => e.Message == "PerformApplicationBackupRequestHandler_HandleAsync_CreatingArchive")))
            .Returns(() => {
                var temporaryRoot = SysPath.Combine(sut.Root.Path, "temporary");
                var runDirectory = SysDirectory.GetDirectories(temporaryRoot).Single();

                lockedCopy = SysFile.Open(
                    SysPath.Combine(runDirectory, "app.config"),
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None
                );

                return Task.CompletedTask;
            });

        try {
            // act & assert
            await Assert.ThrowsAnyAsync<IOException>(
                () => sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken)
            );
        }
        finally {
            lockedCopy?.Dispose();
        }

        var backupsDirectory = SysPath.Combine(sut.Root.Path, "backups");
        Assert.True(!SysDirectory.Exists(backupsDirectory) || SysDirectory.GetFiles(backupsDirectory).Length == 0);

        // Temp-directory cleanup is not asserted here: the lock this test
        // holds on a file inside that directory is exactly what makes
        // DeleteTempDirectory's own best-effort cleanup fail too (it
        // swallows IOException the same way production would if a real
        // external process held the lock), so the directory is only
        // removable once lockedCopy is disposed above.
        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<BackupRecoveryProgressNotificationEvent>(
                e => e.Message == "PerformApplicationBackupRequestHandler_HandleAsync_CreatingArchive_Failure"
                     && e.Type == NotificationType.Failure)),
            Times.Once
        );
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_WhenCancellationRequested_PropagatesAndCleansUpTemporaryDirectory() {
        // arrange
        using var sut = CreateSut();

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "value");

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var request = new PerformApplicationBackupRequest();

        // act & assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.Handler.HandleAsync(request, cts.Token)
        );

        var temporaryRoot = SysPath.Combine(sut.Root.Path, "temporary");
        Assert.True(!SysDirectory.Exists(temporaryRoot) || SysDirectory.GetDirectories(temporaryRoot).Length == 0);
    }
}
