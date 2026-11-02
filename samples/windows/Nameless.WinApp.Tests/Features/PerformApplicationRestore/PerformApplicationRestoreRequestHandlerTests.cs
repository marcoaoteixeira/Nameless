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
        public required Mock<IAppConfigurationManager> AppConfigurationManagerMock { get; init; }
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

        var appConfigurationManagerMock = CreateAppConfigurationManagerMock();

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
            appConfigurationManager: appConfigurationManagerMock.Object,
            eventPublisher: eventPublisherMock.Object,
            translator: new FakeTranslator()
        );

        return new Sut {
            Root = root,
            FileManager = fileManager,
            IndexProviderMock = indexProviderMock,
            EventPublisherMock = eventPublisherMock,
            AppConfigurationManagerMock = appConfigurationManagerMock,
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

    // Builds a backup-shaped zip from whatever entries the caller stages,
    // independent of the handler's own notion of "current live state" —
    // lets tests build archives that are missing entries on purpose.
    private static string CreateZipArchive(TempDirectory root, string zipName, Action<string> populateStagingDirectory) {
        var stagingDirectoryPath = SysPath.Combine(root.Path, $"{zipName}-staging");
        SysDirectory.CreateDirectory(stagingDirectoryPath);

        populateStagingDirectory(stagingDirectoryPath);

        var zipPath = SysPath.Combine(root.Path, zipName);

        ZipFile.CreateFromDirectory(stagingDirectoryPath, zipPath);

        return zipPath;
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
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "original-value");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        var luceneFilePath = SysPath.Combine(luceneDirectoryPath, "segments.gen");
        SysFile.WriteAllText(luceneFilePath, "original-segment");

        // Not a valid zip at all: ZipFile.ExtractToDirectoryAsync throws
        // InvalidDataException while reading its central directory — before
        // anything is ever renamed away.
        var corruptZipPath = SysPath.Combine(sut.Root.Path, "corrupt.zip");
        SysFile.WriteAllText(corruptZipPath, "this is not a zip file");

        var request = new PerformApplicationRestoreRequest(corruptZipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);

        Assert.Equal("""{"theme":"original"}""", SysFile.ReadAllText(configPath));
        Assert.Equal("original-value", ReadSqliteValue(databasePath));
        Assert.Equal("original-segment", SysFile.ReadAllText(luceneFilePath));

        // No rollback siblings left lying around next to the restored originals.
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.IndexProviderMock.Verify(x => x.CloseAll(), Times.Once);
        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Never);

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Once
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenZipHasNoRecognizedEntries_RollsBackWithOriginalsIntact() {
        // arrange
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var zipPath = CreateZipArchive(sut.Root, "unrecognized.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "readme.txt"), "nothing useful here"));

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        Assert.Equal("""{"theme":"original"}""", SysFile.ReadAllText(configPath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Never);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenConfigEntryIsNotValidJson_RollsBackAndLeavesOriginalsUntouched() {
        // arrange
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "original-value");

        var zipPath = CreateZipArchive(sut.Root, "bad-config.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.config"), "not json at all"));

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert: validated before anything was swapped, so nothing to roll back.
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        Assert.Equal("""{"theme":"original"}""", SysFile.ReadAllText(configPath));
        Assert.Equal("original-value", ReadSqliteValue(databasePath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Never);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenDatabaseEntryIsNotValidSqlite_RollsBackAndLeavesOriginalsUntouched() {
        // arrange
        using var sut = CreateSut();

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "original-value");

        var zipPath = CreateZipArchive(sut.Root, "bad-database.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.db"), "not a sqlite file"));

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        Assert.Equal("original-value", ReadSqliteValue(databasePath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenZipOnlyHasASubsetOfEntries_RestoresThatEntryAndLeavesTheOthersAsIs() {
        // arrange: the zip only backed up app.config — mirrors a backup
        // taken before the database/Lucene index ever existed.
        using var sut = CreateSut();

        SysFile.WriteAllText(SysPath.Combine(sut.Root.Path, "app.config"), """{"theme":"stale"}""");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "untouched-value");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        var luceneFilePath = SysPath.Combine(luceneDirectoryPath, "segments.gen");
        SysFile.WriteAllText(luceneFilePath, "untouched-segment");

        var zipPath = CreateZipArchive(sut.Root, "config-only.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.config"), """{"theme":"restored"}"""));

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.Completed, outcome);
        Assert.Equal("""{"theme":"restored"}""", SysFile.ReadAllText(SysPath.Combine(sut.Root.Path, "app.config")));

        // Entries the zip did not include are left exactly as they were —
        // not renamed away, not deleted.
        Assert.Equal("untouched-value", ReadSqliteValue(databasePath));
        Assert.Equal("untouched-segment", SysFile.ReadAllText(luceneFilePath));

        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Once);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenAnArtifactIsAlreadySwappedAndALaterOneFails_UndoesTheSwapAndRollsBack() {
        // arrange: both app.config and app.db are in the archive. app.config
        // swaps successfully; the live app.db is locked open so the handler
        // cannot even rename it away, forcing a failure after config has
        // already been put in its final place.
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "original-value");

        var zipPath = CreateZipArchive(sut.Root, "config-and-db.zip", stagingPath => {
            SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.config"), """{"theme":"restored"}""");
            CreateSqliteDatabase(SysPath.Combine(stagingPath, "app.db"), "restored-value");
        });

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        RestoreOutcome outcome;
        using (var databaseLock = new FileStream(databasePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
            outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);
        }

        // assert: the config swap that already happened was undone too.
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        Assert.Equal("""{"theme":"original"}""", SysFile.ReadAllText(configPath));
        Assert.Equal("original-value", ReadSqliteValue(databasePath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Never);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenCancelled_RollsBackAndReturnsRolledBackInsteadOfThrowing() {
        // arrange
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var zipPath = CreateZipArchive(sut.Root, "any.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.config"), """{"theme":"restored"}"""));

        var request = new PerformApplicationRestoreRequest(zipPath);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // act
        var outcome = await sut.Handler.HandleAsync(request, cts.Token);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        Assert.Equal("""{"theme":"original"}""", SysFile.ReadAllText(configPath));
        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        sut.EventPublisherMock.Verify(
            x => x.PublishAsync(It.Is<SnackBarNotificationEvent>(e => e.Type == NotificationType.Failure)),
            Times.Once
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenSuccessPublishFailsAfterConfigSwap_NeverReloadsAheadOfTheRollback() {
        // arrange: app.config swaps successfully and the rollback-backup
        // cleanup completes, but the final "Success" progress publish
        // itself throws — a late failure on the path that used to run
        // after Reload(). The outer catch still treats this as a failed
        // restore and runs the (best-effort) rollback.
        //
        // Note: by this point DeleteBackupSiblingsAsync has already
        // deleted the rollback-point backup, so the rollback itself cannot
        // actually restore app.config's original bytes here — a separate,
        // pre-existing gap (cleanup runs before the still-failable final
        // publish) that is not what this test is pinning. What matters for
        // this fix is that Reload() is never called on this path: calling
        // it here would make the in-memory config hold the (soon to be
        // undone) restored values even though the restore is reported as
        // RolledBack — the exact stale-state exposure the reorder fixes.
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"theme":"original"}""");

        var zipPath = CreateZipArchive(sut.Root, "config-only.zip",
            stagingPath => SysFile.WriteAllText(SysPath.Combine(stagingPath, "app.config"), """{"theme":"restored"}"""));

        sut.EventPublisherMock
            .Setup(x => x.PublishAsync(It.Is<BackupRecoveryProgressNotificationEvent>(
                e => e.Message == "PerformApplicationRestoreRequestHandler_HandleAsync_Success")))
            .ThrowsAsync(new InvalidOperationException("event bus is down"));

        var request = new PerformApplicationRestoreRequest(zipPath);

        // act
        var outcome = await sut.Handler.HandleAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(RestoreOutcome.RolledBack, outcome);
        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Never);
    }

    [Fact]
    [IntegrationTest]
    public async Task HandleAsync_RoundTrip_RestoresConfigDatabaseAndLuceneIndexFromBackup() {
        // arrange: back up a real small SQLite database and a fake index
        // folder using the already-shipped backup handler, then restore
        // from that archive and verify every file round-trips.
        using var sut = CreateSut();

        SysFile.WriteAllText(SysPath.Combine(sut.Root.Path, "app.config"), """{"theme":"round-trip"}""");

        var databasePath = SysPath.Combine(sut.Root.Path, "databases", "app.db");
        CreateSqliteDatabase(databasePath, "round-trip-value");

        var luceneDirectoryPath = SysPath.Combine(sut.Root.Path, "lucene");
        SysDirectory.CreateDirectory(luceneDirectoryPath);
        SysFile.WriteAllText(SysPath.Combine(luceneDirectoryPath, "segments.gen"), "round-trip-segment");

        var backupHandler = new PerformApplicationBackupRequestHandler(
            applicationContext: CreateApplicationContextMock(sut.FileManager).Object,
            appConfigurationManager: sut.AppConfigurationManagerMock.Object,
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

        Assert.Equal("""{"theme":"round-trip"}""", SysFile.ReadAllText(SysPath.Combine(sut.Root.Path, "app.config")));
        Assert.Equal("round-trip-value", ReadSqliteValue(databasePath));
        Assert.Equal("round-trip-segment", SysFile.ReadAllText(SysPath.Combine(luceneDirectoryPath, "segments.gen")));

        Assert.DoesNotContain(SysDirectory.GetFileSystemEntries(sut.Root.Path), entry => entry.Contains(".bak-", StringComparison.Ordinal));

        // The critical fix: the in-memory configuration manager must be
        // reloaded from the restored file so a later shutdown-time
        // SaveChangesAsync does not clobber it with stale pre-restore data.
        sut.AppConfigurationManagerMock.Verify(x => x.Reload(), Times.Once);
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
