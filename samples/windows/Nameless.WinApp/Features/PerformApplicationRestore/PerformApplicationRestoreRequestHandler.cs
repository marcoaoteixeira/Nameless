using System.IO;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nameless.Contracts;
using Nameless.IO;
using Nameless.Lucene;
using Nameless.Mediator.Requests;
using Nameless.WinApp.Data;
using Nameless.Windows;
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
    // Sibling of the original artifact used as the rollback point. Tracks
    // enough to both move it back into place and to know whether it is a
    // file or a directory (file/directory system APIs differ).
    private readonly record struct RenamedArtifact(string OriginalPath, string BackupPath, bool IsDirectory);

    private readonly IFileManager _fileManager;
    private readonly AppDbContext _dbContext;
    private readonly IIndexProvider _indexProvider;
    private readonly IOptions<LuceneOptions> _luceneOptions;
    private readonly IEventPublisher _eventPublisher;
    private readonly ITranslator _translator;

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
    /// <param name="eventPublisher">
    ///     The event publisher.
    /// </param>
    /// <param name="translator">
    ///     The translator.
    /// </param>
    public PerformApplicationRestoreRequestHandler(
        IFileManager fileManager,
        AppDbContext dbContext,
        IIndexProvider indexProvider,
        IOptions<LuceneOptions> luceneOptions,
        IEventPublisher eventPublisher,
        ITranslator translator) {
        _fileManager = Throws.When.Null(fileManager);
        _dbContext = Throws.When.Null(dbContext);
        _indexProvider = Throws.When.Null(indexProvider);
        _luceneOptions = Throws.When.Null(luceneOptions);
        _eventPublisher = Throws.When.Null(eventPublisher);
        _translator = Throws.When.Null(translator);
    }

    /// <inheritdoc />
    public async Task<RestoreOutcome> HandleAsync(PerformApplicationRestoreRequest request, CancellationToken cancellationToken) {
        Throws.When.Null(request);

        var timestamp = DateTimeOffset.UtcNow;
        var renamedArtifacts = new List<RenamedArtifact>();
        IDirectory? extractedDirectory = null;

        try {
            await PublishProgressAsync("Starting");

            await RunStepAsync("ClosingDatabaseConnection", () => _dbContext.Database.CloseConnectionAsync());

            await RunStepAsync("ClosingLuceneIndex", () => {
                _indexProvider.CloseAll();

                return Task.CompletedTask;
            });

            await RunStepAsync("RenamingExistingArtifacts", () => {
                RenameExistingArtifacts(renamedArtifacts, timestamp);

                return Task.CompletedTask;
            });

            await RunStepAsync("ExtractingArchive", async () => {
                extractedDirectory = await ExtractArchiveAsync(request.SourceZipFilePath, timestamp, cancellationToken);
            });

            await RunStepAsync("RestoringArtifacts", () => {
                RestoreArtifactsFromExtractedDirectory(extractedDirectory!);

                return Task.CompletedTask;
            });

            // Everything swapped successfully: the rollback point is no
            // longer needed.
            DeleteBackupSiblings(renamedArtifacts);

            await PublishProgressAsync("Success");

            return RestoreOutcome.Completed;
        }
        catch (Exception ex) {
            // Best-effort: move whatever was already renamed away back into
            // place so the application can keep running as it was before
            // this attempt.
            await RollbackRenamedArtifactsAsync(renamedArtifacts);

            await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
                message: string.Format(_translator[$"{Prefix}_RolledBack"], ex.Message)
            );

            return RestoreOutcome.RolledBack;
        }
        finally {
            DeleteExtractedDirectory(extractedDirectory);
        }
    }

    private async Task RunStepAsync(string stepName, Func<Task> action) {
        await PublishProgressAsync(stepName);

        try {
            await action();
        }
        catch (Exception ex) {
            await PublishStepFailureAsync(stepName, ex);

            throw;
        }
    }

    private void RenameExistingArtifacts(List<RenamedArtifact> renamedArtifacts, DateTimeOffset timestamp) {
        RenameFileIfExists(_fileManager.GetFile(Constants.BackupRecovery.AppConfigFileName), renamedArtifacts, timestamp);
        RenameDirectoryIfExists(_fileManager.GetDatabaseDirectory(), renamedArtifacts, timestamp);
        RenameDirectoryIfExists(_fileManager.GetDirectory(_luceneOptions.Value.DirectoryRelativePath), renamedArtifacts, timestamp);
    }

    private static void RenameFileIfExists(IFile file, List<RenamedArtifact> renamedArtifacts, DateTimeOffset timestamp) {
        if (!file.Exists) { return; }

        var backupPath = BuildBackupSiblingPath(file.Path, timestamp);

        SysFile.Move(file.Path, backupPath);

        renamedArtifacts.Add(new RenamedArtifact(file.Path, backupPath, IsDirectory: false));
    }

    private static void RenameDirectoryIfExists(IDirectory directory, List<RenamedArtifact> renamedArtifacts, DateTimeOffset timestamp) {
        if (!directory.Exists) { return; }

        var backupPath = BuildBackupSiblingPath(directory.Path, timestamp);

        SysDirectory.Move(directory.Path, backupPath);

        renamedArtifacts.Add(new RenamedArtifact(directory.Path, backupPath, IsDirectory: true));
    }

    private static string BuildBackupSiblingPath(string originalPath, DateTimeOffset timestamp) {
        var parentDirectoryPath = SysPath.GetDirectoryName(originalPath);

        Throws.When.NullOrWhiteSpace(parentDirectoryPath);

        var siblingName = $"{SysPath.GetFileName(originalPath)}.bak-{timestamp:yyyyMMddHHmmss}";

        return SysPath.Combine(parentDirectoryPath, siblingName);
    }

    private async Task<IDirectory> ExtractArchiveAsync(string sourceZipFilePath, DateTimeOffset timestamp, CancellationToken cancellationToken) {
        var extractDirectory = _fileManager.GetTemporaryDirectory(timestamp);

        await ZipFile.ExtractToDirectoryAsync(
            sourceArchiveFileName: sourceZipFilePath,
            destinationDirectoryName: extractDirectory.Path,
            overwriteFiles: false,
            cancellationToken
        );

        return extractDirectory;
    }

    private void RestoreArtifactsFromExtractedDirectory(IDirectory extractedDirectory) {
        var databaseDirectory = _fileManager.GetDatabaseDirectory();
        var luceneDirectory = _fileManager.GetDirectory(_luceneOptions.Value.DirectoryRelativePath);

        var restoredConfig = TryMoveExtractedFile(
            extractedDirectory, Constants.BackupRecovery.AppConfigFileName,
            _fileManager.GetFile(Constants.BackupRecovery.AppConfigFileName).Path
        );

        var restoredDatabase = TryMoveExtractedFile(
            extractedDirectory, Constants.Database.FileName,
            SysPath.Combine(databaseDirectory.Path, Constants.Database.FileName)
        );

        var restoredLuceneIndex = TryMoveExtractedDirectory(
            extractedDirectory, luceneDirectory.Name, luceneDirectory.Path
        );

        if (!restoredConfig && !restoredDatabase && !restoredLuceneIndex) {
            throw new InvalidDataException(
                $"The backup archive '{extractedDirectory.Path}' does not contain any recognizable application artifact."
            );
        }
    }

    private static bool TryMoveExtractedFile(IDirectory extractedDirectory, string entryName, string destinationPath) {
        var sourcePath = SysPath.Combine(extractedDirectory.Path, entryName);

        if (!SysFile.Exists(sourcePath)) { return false; }

        var destinationDirectoryPath = SysPath.GetDirectoryName(destinationPath);

        if (destinationDirectoryPath is not null) { SysDirectory.CreateDirectory(destinationDirectoryPath); }

        SysFile.Move(sourcePath, destinationPath);

        return true;
    }

    private static bool TryMoveExtractedDirectory(IDirectory extractedDirectory, string entryName, string destinationPath) {
        var sourcePath = SysPath.Combine(extractedDirectory.Path, entryName);

        if (!SysDirectory.Exists(sourcePath)) { return false; }

        var destinationParentPath = SysPath.GetDirectoryName(destinationPath);

        if (destinationParentPath is not null) { SysDirectory.CreateDirectory(destinationParentPath); }

        SysDirectory.Move(sourcePath, destinationPath);

        return true;
    }

    private async Task RollbackRenamedArtifactsAsync(List<RenamedArtifact> renamedArtifacts) {
        // Walk back-to-front: later renames are the ones most likely to be
        // sitting next to a partially-restored artifact.
        for (var idx = renamedArtifacts.Count - 1; idx >= 0; idx--) {
            var artifact = renamedArtifacts[idx];

            try {
                if (artifact.IsDirectory) {
                    if (SysDirectory.Exists(artifact.OriginalPath)) { SysDirectory.Delete(artifact.OriginalPath, recursive: true); }

                    SysDirectory.Move(artifact.BackupPath, artifact.OriginalPath);
                } else {
                    if (SysFile.Exists(artifact.OriginalPath)) { SysFile.Delete(artifact.OriginalPath); }

                    SysFile.Move(artifact.BackupPath, artifact.OriginalPath);
                }
            }
            catch (Exception ex) {
                // Best-effort: the user is told (via the SnackBar failure
                // message published right after this call) to check the
                // backups manually if this happens.
                await PublishStepFailureAsync("RollingBack", ex);
            }
        }
    }

    private static void DeleteBackupSiblings(List<RenamedArtifact> renamedArtifacts) {
        foreach (var artifact in renamedArtifacts) {
            try {
                if (artifact.IsDirectory) {
                    if (SysDirectory.Exists(artifact.BackupPath)) { SysDirectory.Delete(artifact.BackupPath, recursive: true); }
                } else if (SysFile.Exists(artifact.BackupPath)) {
                    SysFile.Delete(artifact.BackupPath);
                }
            }
            catch (IOException) { /* best effort */ }
            catch (UnauthorizedAccessException) { /* best effort */ }
        }
    }

    private static void DeleteExtractedDirectory(IDirectory? extractedDirectory) {
        if (extractedDirectory is null) { return; }

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
