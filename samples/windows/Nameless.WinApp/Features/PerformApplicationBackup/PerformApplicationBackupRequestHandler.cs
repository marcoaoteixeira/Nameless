using System.IO;
using System.IO.Compression;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nameless.Application;
using Nameless.Contracts;
using Nameless.IO;
using Nameless.Lucene;
using Nameless.Mediator.Requests;
using Nameless.WinApp.Data;
using Nameless.Windows;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.Notification;
using Nameless.Windows.Notification.BackupRecovery;

namespace Nameless.WinApp.Features.PerformApplicationBackup;

/// <summary>
///     Default handler for <see cref="PerformApplicationBackupRequest"/>.
/// </summary>
public class PerformApplicationBackupRequestHandler : IRequestHandler<PerformApplicationBackupRequest, string> {
    private readonly IApplicationContext _applicationContext;
    private readonly IAppConfigurationManager _appConfigurationManager;
    private readonly AppDbContext _dbContext;
    private readonly IIndexProvider _indexProvider;
    private readonly IOptions<LuceneOptions> _luceneOptions;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITranslator<PerformApplicationBackupRequestHandler> _translator;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PerformApplicationBackupRequestHandler"/> class.
    /// </summary>
    /// <param name="applicationContext">
    ///     The application context.
    /// </param>
    /// <param name="appConfigurationManager">
    ///     The application configuration manager.
    /// </param>
    /// <param name="dbContext">
    ///     The application database context.
    /// </param>
    /// <param name="indexProvider">
    ///     The Lucene index provider.
    /// </param>
    /// <param name="luceneOptions">
    ///     The Lucene options.
    /// </param>
    /// <param name="eventPublisher">
    ///     The event publisher.
    /// </param>
    /// <param name="translator">
    ///     The translator.
    /// </param>
    public PerformApplicationBackupRequestHandler(
        IApplicationContext applicationContext,
        IAppConfigurationManager appConfigurationManager,
        AppDbContext dbContext,
        IIndexProvider indexProvider,
        IOptions<LuceneOptions> luceneOptions,
        IEventPublisher eventPublisher,
        ITranslator<PerformApplicationBackupRequestHandler> translator) {
        _applicationContext = Throws.When.Null(applicationContext);
        _appConfigurationManager = Throws.When.Null(appConfigurationManager);
        _dbContext = Throws.When.Null(dbContext);
        _indexProvider = Throws.When.Null(indexProvider);
        _luceneOptions = Throws.When.Null(luceneOptions);
        _eventPublisher = Throws.When.Null(eventPublisher);
        _translator = Throws.When.Null(translator);
    }

    /// <inheritdoc />
    public async Task<string> HandleAsync(PerformApplicationBackupRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var timestamp = DateTimeOffset.UtcNow;
        var tempDirectory = _applicationContext.FileManager.GetTemporaryDirectory(timestamp);

        try {
            await PublishProgressAsync("Starting");

            // Step 2: settings changed this session only hit disk at app
            // exit otherwise; back up stale or possibly-absent data without this.
            try {
                await _appConfigurationManager.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) {
                await PublishStepFailureAsync("SavingConfiguration", ex);

                throw;
            }

            // Step 3: parity/best-effort — actual consistency comes from
            // the SQLite online-backup API used below, not this call.
            await PublishProgressAsync("ClosingDatabaseConnection");

            try {
                await _dbContext.Database.CloseConnectionAsync();
            }
            catch (Exception ex) {
                await PublishStepFailureAsync("ClosingDatabaseConnection", ex);

                throw;
            }

            // Step 4: per-run temp working directory.
            tempDirectory.Create();

            // Step 5: copy app.config, if it exists.
            try {
                await CopyConfigurationFileAsync(tempDirectory);
            }
            catch (Exception ex) {
                await PublishStepFailureAsync("CopyingConfigurationFile", ex);

                throw;
            }

            // Step 6: SQLite online backup.
            await PublishProgressAsync("BackingUpDatabase");

            try {
                await BackupDatabaseAsync(tempDirectory, cancellationToken);
            }
            catch (Exception ex) {
                await PublishStepFailureAsync("BackingUpDatabase", ex);

                throw;
            }

            // Step 7: close every Lucene index and copy the on-disk files.
            await PublishProgressAsync("BackingUpLuceneIndex");

            try {
                _indexProvider.CloseAll();

                CopyLuceneIndexDirectory(tempDirectory);
            }
            catch (Exception ex) {
                await PublishStepFailureAsync("BackingUpLuceneIndex", ex);

                throw;
            }

            // Step 8: zip everything up.
            await PublishProgressAsync("CreatingArchive");

            var destinationFilePath = GetDestinationFilePath(timestamp);

            try {
                await ZipFile.CreateFromDirectoryAsync(
                    sourceDirectoryName: tempDirectory.Path,
                    destinationArchiveFileName: destinationFilePath,
                    compressionLevel: CompressionLevel.Optimal,
                    includeBaseDirectory: false,
                    cancellationToken
                );
            }
            catch (Exception ex) {
                DeletePartialArchive(destinationFilePath);

                await PublishStepFailureAsync("CreatingArchive", ex);

                throw;
            }

            // Step 9: done.
            await PublishProgressAsync("Success");

            return destinationFilePath;
        }
        finally {
            DeleteTempDirectory(tempDirectory);
        }
    }

    private async Task CopyConfigurationFileAsync(IDirectory tempDirectory) {
        var configFile = _applicationContext.FileManager.GetFile(Constants.BackupRecovery.AppConfigFileName);

        if (!configFile.Exists) {
            await _eventPublisher.PublishWarningAsync<BackupRecoveryProgressNotificationEvent>(
                message: _translator["HandleAsync_CopyingConfigurationFile_Missing"]
            );

            return;
        }

        var destinationPath = SysPath.Combine(tempDirectory.Path, Constants.BackupRecovery.AppConfigFileName);

        SysFile.Copy(configFile.Path, destinationPath, overwrite: true);
    }

    private async Task BackupDatabaseAsync(IDirectory tempDirectory, CancellationToken cancellationToken) {
        var databaseDirectory = _applicationContext.FileManager.GetDatabaseDirectory();
        var databaseFile = _applicationContext.FileManager.GetFile(
            SysPath.Combine(databaseDirectory.Name, Constants.Database.FileName)
        );

        // SqliteConnection.Open() against a missing database file silently
        // creates an empty one, which would then be backed up as if it were
        // real data — treat a missing source as "nothing to back up yet".
        if (!databaseFile.Exists) { return; }

        var destinationDatabasePath = SysPath.Combine(tempDirectory.Path, Constants.Database.FileName);

        var sourceConnectionString = new SqliteConnectionStringBuilder {
            DataSource = databaseFile.Path,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        }.ToString();

        var destinationConnectionString = new SqliteConnectionStringBuilder {
            DataSource = destinationDatabasePath,
            Pooling = false
        }.ToString();

        await using var sourceConnection = new SqliteConnection(sourceConnectionString);
        await using var destinationConnection = new SqliteConnection(destinationConnectionString);

        await sourceConnection.OpenAsync(cancellationToken);
        await destinationConnection.OpenAsync(cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        // Synchronous API: keep the TaskRunner window responsive.
        await Task.Run(() => sourceConnection.BackupDatabase(destinationConnection), cancellationToken);
    }

    private void CopyLuceneIndexDirectory(IDirectory tempDirectory) {
        var luceneDirectory = _applicationContext.FileManager.GetDirectory(_luceneOptions.Value.DirectoryRelativePath);

        // A missing folder means "nothing to back up yet", not an error.
        if (!luceneDirectory.Exists) { return; }

        var destinationLuceneDirectoryPath = SysPath.Combine(tempDirectory.Path, luceneDirectory.Name);

        CopyDirectoryRecursive(luceneDirectory.Path, destinationLuceneDirectoryPath);
    }

    // No existing recursive-copy helper exists in the repo for this; kept
    // private to this handler per the plan (a shared abstraction is only
    // worth it if a second consumer appears).
    private static void CopyDirectoryRecursive(string sourceDirectoryPath, string destinationDirectoryPath) {
        SysDirectory.CreateDirectory(destinationDirectoryPath);

        foreach (var filePath in SysDirectory.EnumerateFiles(sourceDirectoryPath)) {
            var fileName = SysPath.GetFileName(filePath);

            // Skip Lucene's write lock: copying it risks a stale-locked copy.
            if (string.Equals(fileName, "write.lock", StringComparison.OrdinalIgnoreCase)) { continue; }

            SysFile.Copy(filePath, SysPath.Combine(destinationDirectoryPath, fileName), overwrite: true);
        }

        foreach (var directoryPath in SysDirectory.EnumerateDirectories(sourceDirectoryPath)) {
            var directoryName = SysPath.GetFileName(directoryPath);

            CopyDirectoryRecursive(directoryPath, SysPath.Combine(destinationDirectoryPath, directoryName));
        }
    }

    private string GetDestinationFilePath(DateTimeOffset timestamp) {
        var backupDirectory = _applicationContext.FileManager.GetBackupDirectory();

        backupDirectory.Create();

        var fileName = string.Format(Constants.BackupRecovery.FileNamePattern, timestamp);

        return SysPath.Combine(backupDirectory.Path, fileName);
    }

    private static void DeletePartialArchive(string destinationFilePath) {
        try {
            if (SysFile.Exists(destinationFilePath)) {
                SysFile.Delete(destinationFilePath);
            }
        }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private static void DeleteTempDirectory(IDirectory tempDirectory) {
        try {
            if (tempDirectory.Exists) {
                tempDirectory.Delete(recursive: true);
            }
        }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private Task PublishProgressAsync(string suffix) {
        return _eventPublisher.PublishInformationAsync<BackupRecoveryProgressNotificationEvent>(
            message: _translator[$"HandleAsync_{suffix}"]
        );
    }

    private Task PublishStepFailureAsync(string suffix, Exception exception) {
        return _eventPublisher.PublishFailureAsync<BackupRecoveryProgressNotificationEvent>(
            message: string.Format(_translator[$"HandleAsync_{suffix}_Failure"], exception.Message)
        );
    }
}
