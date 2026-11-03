using System.IO;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using Nameless.Windows.Notification.SnackBar;

namespace Nameless.WinApp.Features.PerformApplicationRestore;

/// <summary>
///     Default handler for <see cref="PerformApplicationRestoreRequest"/>.
/// </summary>
public class PerformApplicationRestoreRequestHandler : IRequestHandler<PerformApplicationRestoreRequest, RestoreOutcome> {
    // One recognized artifact that was actually swapped into place during
    // this run. BackupPath is null when the destination did not exist
    // before the swap (nothing to rename away) — rollback for that case is
    // "delete what was placed", not "move a backup sibling back".
    private readonly record struct ArtifactSwap(string OriginalPath, string? BackupPath, bool IsDirectory);

    // Which of the three recognized entries the (already validated)
    // archive actually contains.
    private readonly record struct ValidatedArchiveContents(bool ConfigPresent, bool DatabasePresent, bool LucenePresent);

    private readonly IFileManager _fileManager;
    private readonly AppDbContext _dbContext;
    private readonly IIndexProvider _indexProvider;
    private readonly IOptions<LuceneOptions> _luceneOptions;
    private readonly IAppConfigurationManager _appConfigurationManager;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITranslator _translator;
    private readonly ILogger<PerformApplicationRestoreRequestHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="PerformApplicationRestoreRequestHandler"/> class.
    /// </summary>
    /// <param name="fileManager">
    ///     The application file manager.
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
    /// <param name="appConfigurationManager">
    ///     The application configuration manager.
    /// </param>
    /// <param name="eventPublisher">
    ///     The event publisher.
    /// </param>
    /// <param name="translator">
    ///     The translator.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public PerformApplicationRestoreRequestHandler(
        IFileManager fileManager,
        AppDbContext dbContext,
        IIndexProvider indexProvider,
        IOptions<LuceneOptions> luceneOptions,
        IAppConfigurationManager appConfigurationManager,
        IEventPublisher eventPublisher,
        ITranslator translator,
        ILogger<PerformApplicationRestoreRequestHandler> logger) {
        _fileManager = Throws.When.Null(fileManager);
        _dbContext = Throws.When.Null(dbContext);
        _indexProvider = Throws.When.Null(indexProvider);
        _luceneOptions = Throws.When.Null(luceneOptions);
        _appConfigurationManager = Throws.When.Null(appConfigurationManager);
        _eventPublisher = Throws.When.Null(eventPublisher);
        _translator = Throws.When.Null(translator);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public async Task<RestoreOutcome> HandleAsync(PerformApplicationRestoreRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var timestamp = DateTimeOffset.UtcNow;
        var swaps = new List<ArtifactSwap>();

        // Resolved up front (cheap — this does not touch disk) so the
        // `finally` below can always find and clean it up, even if
        // extraction itself fails partway through (truncated zip, zip-slip
        // IOException, cancellation).
        var extractedDirectory = _fileManager.GetTemporaryDirectory(timestamp);

        // Once true, nothing below can turn into a rollback: the restored
        // artifacts (and the in-memory configuration, reloaded right before
        // this flips) are the new source of truth. See the catch filters
        // below and the guarded tail that follows the commit.
        var committed = false;

        try {
            await PublishProgressAsync("Starting");

            await RunStepAsync("ClosingDatabaseConnection", () => _dbContext.Database.CloseConnectionAsync());

            await RunStepAsync("ClosingLuceneIndex", () => {
                _indexProvider.CloseAll();

                return Task.CompletedTask;
            });

            await RunStepAsync("ExtractingArchive",
                () => ExtractArchiveAsync(request.SourceZipFilePath, extractedDirectory, cancellationToken));

            var validated = default(ValidatedArchiveContents);

            await RunStepAsync("ValidatingArchive", async () => {
                validated = await ValidateExtractedArchiveAsync(extractedDirectory, cancellationToken);
            });

            // Only swap the artifacts the (now validated) archive actually
            // contains — a backup missing e.g. the Lucene index (nothing to
            // back up yet at the time) must not wipe out the live one.
            await RunStepAsync("RestoringArtifacts",
                () => RestoreValidatedArtifactsAsync(validated, extractedDirectory, timestamp, swaps));

            // The in-memory configuration dictionary would otherwise still
            // hold the pre-restore values; without this, the next
            // SaveChangesAsync (e.g. the one the host runs on application
            // shutdown) clobbers the just-restored app.config. This is the
            // last action that is allowed to roll the restore back: if it
            // throws, the `.bak-*` backups still exist and the catch blocks
            // below undo the swap fully, with disk and memory left on the
            // original values.
            if (validated.ConfigPresent) {
                _appConfigurationManager.Reload();
            }

            // Commit point: once set, a late failure must never delete the
            // restored artifacts or report RolledBack for a restore that
            // already succeeded. Everything from here on runs in its own
            // best-effort guard (CompleteCommittedRestoreAsync) and the
            // catch blocks below are filtered out by `when (!committed)`.
            committed = true;

            await CompleteCommittedRestoreAsync(swaps);

            return RestoreOutcome.Completed;
        }
        catch (OperationCanceledException) when (!committed) {
            await RollbackSwapsAsync(swaps);

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: _translator[$"{Prefix}_RolledBack_Cancelled"]
            );

            return RestoreOutcome.RolledBack;
        }
        catch (Exception ex) when (!committed) {
            // Best-effort: undo whatever was already swapped into place so
            // the application can keep running as it was before this
            // attempt.
            await RollbackSwapsAsync(swaps);

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: string.Format(_translator[$"{Prefix}_RolledBack"], ex.Message)
            );

            return RestoreOutcome.RolledBack;
        }
        finally {
            DeleteExtractedDirectory(extractedDirectory);
        }
    }

    // Runs everything that happens after the commit point. Every step is
    // wrapped in its own best-effort guard: a failure here is cosmetic (the
    // restore already succeeded) and must never escape as an exception or
    // be mistaken for a reason to roll back.
    private async Task CompleteCommittedRestoreAsync(List<ArtifactSwap> swaps) {
        try {
            // Everything swapped successfully: the rollback point is no
            // longer needed.
            await DeleteBackupSiblingsAsync(swaps);
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, $"Post-commit backup-sibling cleanup failed: {ex.Message}", ex, tag: GetType().Tag);
        }

        try {
            await PublishProgressAsync("Success");
        }
        catch (Exception ex) {
            CommonLog.Error(_logger, $"Post-commit success notification failed: {ex.Message}", ex, tag: GetType().Tag);
        }
    }

    private async Task RunStepAsync(string stepName, Func<Task> action) {
        await PublishProgressAsync(stepName);

        try {
            await action();
        }
        catch (Exception ex) when (ex is not OperationCanceledException) {
            await PublishStepFailureAsync(stepName, ex);

            throw;
        }
    }

    private static async Task ExtractArchiveAsync(string sourceZipFilePath, IDirectory extractDirectory, CancellationToken cancellationToken) {
        await ZipFile.ExtractToDirectoryAsync(
            sourceArchiveFileName: sourceZipFilePath,
            destinationDirectoryName: extractDirectory.Path,
            overwriteFiles: false,
            cancellationToken
        );
    }

    private async Task<ValidatedArchiveContents> ValidateExtractedArchiveAsync(IDirectory extractedDirectory, CancellationToken cancellationToken) {
        var luceneDirectory = _fileManager.GetDirectory(_luceneOptions.Value.DirectoryRelativePath);

        var configPath = SysPath.Combine(extractedDirectory.Path, Constants.BackupRecovery.AppConfigFileName);
        var databasePath = SysPath.Combine(extractedDirectory.Path, Constants.Database.FileName);
        var lucenePath = SysPath.Combine(extractedDirectory.Path, luceneDirectory.Name);

        var configPresent = SysFile.Exists(configPath);
        var databasePresent = SysFile.Exists(databasePath);
        var lucenePresent = SysDirectory.Exists(lucenePath);

        if (!configPresent && !databasePresent && !lucenePresent) {
            throw new InvalidDataException(
                $"The backup archive '{extractedDirectory.Path}' does not contain any recognizable application artifact."
            );
        }

        if (configPresent) { ValidateConfigurationFile(configPath); }

        if (databasePresent) { await ValidateSqliteDatabaseAsync(databasePath, cancellationToken); }

        return new ValidatedArchiveContents(configPresent, databasePresent, lucenePresent);
    }

    private static void ValidateConfigurationFile(string configPath) {
        try {
            using var stream = SysFile.OpenRead(configPath);
            using var _ = JsonDocument.Parse(stream);
        }
        catch (JsonException ex) {
            throw new InvalidDataException(
                $"The backup archive's '{Constants.BackupRecovery.AppConfigFileName}' entry is not valid JSON.", ex
            );
        }
    }

    private static async Task ValidateSqliteDatabaseAsync(string databasePath, CancellationToken cancellationToken) {
        var connectionString = new SqliteConnectionStringBuilder {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        }.ToString();

        await using var connection = new SqliteConnection(connectionString);

        try {
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA quick_check;";

            var result = await Task.Run(() => (string?)command.ExecuteScalar(), cancellationToken);

            if (!string.Equals(result, "ok", StringComparison.OrdinalIgnoreCase)) {
                throw new InvalidDataException(
                    $"The backup archive's '{Constants.Database.FileName}' entry failed SQLite's integrity check: {result}."
                );
            }
        }
        catch (SqliteException ex) {
            throw new InvalidDataException(
                $"The backup archive's '{Constants.Database.FileName}' entry is not a valid SQLite database.", ex
            );
        }
    }

    private async Task RestoreValidatedArtifactsAsync(
        ValidatedArchiveContents validated, IDirectory extractedDirectory, DateTimeOffset timestamp, List<ArtifactSwap> swaps) {
        if (validated.ConfigPresent) {
            var extractedPath = SysPath.Combine(extractedDirectory.Path, Constants.BackupRecovery.AppConfigFileName);
            var destinationPath = _fileManager.GetFile(Constants.BackupRecovery.AppConfigFileName).Path;

            await SwapFileArtifactAsync(extractedPath, destinationPath, timestamp, swaps);
        }

        if (validated.DatabasePresent) {
            var databaseDirectory = _fileManager.GetDatabaseDirectory();
            var extractedPath = SysPath.Combine(extractedDirectory.Path, Constants.Database.FileName);
            var destinationPath = SysPath.Combine(databaseDirectory.Path, Constants.Database.FileName);

            await SwapFileArtifactAsync(extractedPath, destinationPath, timestamp, swaps);
        }

        if (validated.LucenePresent) {
            var luceneDirectory = _fileManager.GetDirectory(_luceneOptions.Value.DirectoryRelativePath);
            var extractedPath = SysPath.Combine(extractedDirectory.Path, luceneDirectory.Name);

            await SwapDirectoryArtifactAsync(extractedPath, luceneDirectory.Path, timestamp, swaps);
        }
    }

    private async Task SwapFileArtifactAsync(string extractedPath, string destinationPath, DateTimeOffset timestamp, List<ArtifactSwap> swaps) {
        string? backupPath = null;

        if (SysFile.Exists(destinationPath)) {
            backupPath = BuildBackupSiblingPath(destinationPath, timestamp);

            SysFile.Move(destinationPath, backupPath);
        }

        try {
            var destinationDirectoryPath = SysPath.GetDirectoryName(destinationPath);

            if (destinationDirectoryPath is not null) { SysDirectory.CreateDirectory(destinationDirectoryPath); }

            SysFile.Move(extractedPath, destinationPath);
        }
        catch {
            // Self-heal this one artifact immediately: put its own backup
            // back before letting the caller's outer rollback deal with
            // whatever else was already swapped. A failure here must not
            // mask the exception driving the rollback (rethrown below) —
            // it is contained to a warning instead.
            if (backupPath is not null) { await SelfHealAsync(() => SysFile.Move(backupPath, destinationPath), destinationPath); }

            throw;
        }

        swaps.Add(new ArtifactSwap(destinationPath, backupPath, IsDirectory: false));
    }

    private async Task SwapDirectoryArtifactAsync(string extractedPath, string destinationPath, DateTimeOffset timestamp, List<ArtifactSwap> swaps) {
        string? backupPath = null;

        if (SysDirectory.Exists(destinationPath)) {
            backupPath = BuildBackupSiblingPath(destinationPath, timestamp);

            SysDirectory.Move(destinationPath, backupPath);
        }

        try {
            var destinationParentPath = SysPath.GetDirectoryName(destinationPath);

            if (destinationParentPath is not null) { SysDirectory.CreateDirectory(destinationParentPath); }

            SysDirectory.Move(extractedPath, destinationPath);
        }
        catch {
            if (backupPath is not null) { await SelfHealAsync(() => SysDirectory.Move(backupPath, destinationPath), destinationPath); }

            throw;
        }

        swaps.Add(new ArtifactSwap(destinationPath, backupPath, IsDirectory: true));
    }

    private async Task SelfHealAsync(Action moveBackupBack, string destinationPath) {
        try {
            moveBackupBack();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            // Best-effort: the outer rollback (and the "RolledBack"/
            // "RolledBack_Cancelled" SnackBar message it publishes) still
            // runs against the original exception that triggered this
            // self-heal attempt. The `.bak-*` sibling this leaves behind is
            // called out explicitly so it is not unaccounted for.
            await _eventPublisher.PublishWarningAsync<BackupRecoveryProgressNotificationEvent>(
                message: string.Format(_translator[$"{Prefix}_SelfHeal_Failure"], destinationPath, ex.Message)
            );
        }
    }

    private static string BuildBackupSiblingPath(string originalPath, DateTimeOffset timestamp) {
        var parentDirectoryPath = SysPath.GetDirectoryName(originalPath);

        Throws.When.NullOrWhiteSpace(parentDirectoryPath);

        var siblingName = $"{SysPath.GetFileName(originalPath)}.bak-{timestamp:yyyyMMddHHmmss}";

        return SysPath.Combine(parentDirectoryPath, siblingName);
    }

    private async Task RollbackSwapsAsync(List<ArtifactSwap> swaps) {
        // Walk back-to-front: later swaps are the ones most likely to be
        // sitting next to a partially-restored artifact.
        for (var idx = swaps.Count - 1; idx >= 0; idx--) {
            var swap = swaps[idx];

            try {
                if (swap.IsDirectory) {
                    if (SysDirectory.Exists(swap.OriginalPath)) { SysDirectory.Delete(swap.OriginalPath, recursive: true); }

                    // No backup sibling means the artifact did not exist
                    // before this run — nothing to move back, the delete
                    // above already undid it.
                    if (swap.BackupPath is not null) { SysDirectory.Move(swap.BackupPath, swap.OriginalPath); }
                } else {
                    if (SysFile.Exists(swap.OriginalPath)) { SysFile.Delete(swap.OriginalPath); }

                    if (swap.BackupPath is not null) { SysFile.Move(swap.BackupPath, swap.OriginalPath); }
                }
            }
            catch (Exception ex) {
                // Best-effort: the user is told (via the SnackBar failure
                // message published right after this call) to check the
                // application folder manually if this happens.
                await PublishStepFailureAsync("RollingBack", ex);
            }
        }
    }

    private async Task DeleteBackupSiblingsAsync(List<ArtifactSwap> swaps) {
        foreach (var swap in swaps) {
            if (swap.BackupPath is null) { continue; }

            try {
                if (swap.IsDirectory) {
                    if (SysDirectory.Exists(swap.BackupPath)) { SysDirectory.Delete(swap.BackupPath, recursive: true); }
                } else if (SysFile.Exists(swap.BackupPath)) {
                    SysFile.Delete(swap.BackupPath);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
                // Not fatal to the restore itself (it already succeeded) —
                // but the orphaned sibling should not disappear silently.
                // The publish itself is wrapped separately: if the
                // publisher is also down, that must not stop the loop from
                // attempting to clean up the remaining siblings.
                try {
                    await _eventPublisher.PublishWarningAsync<BackupRecoveryProgressNotificationEvent>(
                        message: string.Format(_translator[$"{Prefix}_DeletingBackupSiblings_Failure"], swap.BackupPath, ex.Message)
                    );
                }
                catch (Exception publishEx) {
                    CommonLog.Error(_logger, $"Backup-sibling cleanup warning publish failed for '{swap.BackupPath}': {publishEx.Message}", publishEx, tag: GetType().Tag);
                }
            }
        }
    }

    private static void DeleteExtractedDirectory(IDirectory extractedDirectory) {
        try {
            if (extractedDirectory.Exists) { extractedDirectory.Delete(recursive: true); }
        }
        catch (IOException) { /* best effort */ }
        catch (UnauthorizedAccessException) { /* best effort */ }
    }

    private Task PublishProgressAsync(string suffix) {
        return _eventPublisher.PublishInformationAsync<BackupRecoveryProgressNotificationEvent>(
            message: _translator[$"{Prefix}_{suffix}"]
        );
    }

    private Task PublishStepFailureAsync(string suffix, Exception exception) {
        return _eventPublisher.PublishFailureAsync<BackupRecoveryProgressNotificationEvent>(
            message: string.Format(_translator[$"{Prefix}_{suffix}_Failure"], exception.Message)
        );
    }

    private static string Prefix => $"{nameof(PerformApplicationRestoreRequestHandler)}_{nameof(HandleAsync)}";
}
