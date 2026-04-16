using System.IO;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.IO.FileSystem;
using Nameless.Lucene;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.WPF.Client.Lucene.Resources;
using Nameless.WPF.DisasterRecovery;
using Nameless.WPF.Helpers;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.Client.Lucene.DisasterRecovery;

public class LuceneDisasterRecoveryRoutine : DisasterRecoveryRoutineBase<LuceneDisasterRecoveryRoutine> {
    private readonly IApplicationContext _applicationContext;
    private readonly IIndexProvider _indexProvider;

    public override string Name => Strings.LuceneDisasterRecoveryRoutine_Name;

    public LuceneDisasterRecoveryRoutine(IApplicationContext applicationContext, IIndexProvider indexProvider, IMessenger messenger, ILogger<LuceneDisasterRecoveryRoutine> logger)
        : base(messenger, logger) {
        _applicationContext = applicationContext;
        _indexProvider = indexProvider;
    }

    public override async Task<BackupOutput> BackupAsync(BackupInput input, CancellationToken cancellationToken) {
        await NotifyInformationAsync(
            Strings.LuceneDisasterRecoveryRoutine_Report_Backup_Starting
        ).SkipContextSync();

        // 1) Get source directory
        var sourceDirectory = GetApplicationLuceneDirectoryAsync();

        // 2) Get destination directory
        var relativeDestinat
        var destinationDirectory = _applicationContext.DataDirectory
                                                      .GetDirectory(input.TemporaryDirectoryPath)


        // 3) Backup all files to the destination directory
        var databaseBackupResult = ExecuteDatabaseBackup(
            input.TemporaryDirectoryPath,
            cancellationToken
        );

        // If something goes wrong, just notify and return the error.
        if (databaseBackupResult.Failure) {
            await NotifyFailureAsync(
                string.Format(
                    Strings.LuceneDisasterRecoveryRoutine_Message_Failure,
                    databaseBackupResult.Errors.Flatten()
                )
            ).SkipContextSync();

            return databaseBackupResult.Errors;
        }

        // If the file path is empty, it means we can skip the backup.
        if (databaseBackupResult.Value.Length == 0) {
            await NotifyInformationAsync(
                Strings.LuceneDisasterRecoveryRoutine_Message_Ignore
            ).SkipContextSync();

            return BackupOutput.Empty;
        }

        // Ok, everything is fine.
        await NotifySuccessAsync(
            Strings.LuceneDisasterRecoveryRoutine_Message_Success
        ).SkipContextSync();

        return databaseBackupResult.Value;
    }

    public override Task<RestoreOutput> RestoreAsync(RestoreInput input, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    private Result<string[]> ExecuteLuceneDatabaseBackup(string sourceDirectoryPath, string destinationDirectoryPath, CancellationToken cancellationToken) {
        var sourceDirectory = _applicationContext.DataDirectory.GetDirectory(sourceDirectoryPath);
        var destinationDirectory = _applicationContext.DataDirectory.GetDirectory(destinationDirectoryPath);

        // 1) Dispose the current index to ensure all data is flushed to disk.
        // This is crucial before performing a backup. Also, it will remove the
        // index from the index provider's cache.
        _indexProvider.Get(LuceneConstants.UniqueIndexName).Dispose();

        // 2) Delete all content from the destination to make sure we have a slate
        // start point.
        destinationDirectory.Delete(recursive: true);

        // 3) Recreate the destination directory
        destinationDirectory.Create();

        // 4) Copy all files from the source to the destination
        try
        {
            DirectoryHelper.CopyDirectory(
                sourceDirectory.Path,
                destinationDirectory.Path,
                cancellationToken
            );
        }
        catch (Exception ex) {
            Logger.BackupFailure(ex);

            return Error.Failure(ex.Message);
        }

        // Finally, return all files in the destination directory
        return destinationDirectory.GetFiles(recursive: true)
                                   .Select(item => item.Path)
                                   .ToArray();
    }

    private Result<string[]> ExecuteDatabaseBackup(string destinationDirectoryPath, CancellationToken cancellationToken) {
        var applicationLuceneDirectory = GetApplicationLuceneDirectoryAsync();

        // If the source directory has no files, that means the index was not
        // initialized yet. So, we can just skip the backup.
        if (applicationLuceneDirectory is null || !applicationLuceneDirectory.GetFiles().Any()) {
            return Array.Empty<string>();
        }

        var destinationDirectory = GetDestinationDirectory(
            destinationDirectoryPath
        );

        try
        {
            DirectoryHelper.CopyDirectory(
                applicationLuceneDirectory.Path,
                destinationDirectory.Path,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            Logger.BackupFailure(ex);

            return Error.Failure(ex.Message);
        }

        var files = _applicationContext.DataDirectory
                                       .GetDirectory(destinationDirectory.Path)
                                       .GetFiles()
                                       .Select(file => file.Path)
                                       .ToArray();

        return files;
    }

    private async Task<IDirectory> GetApplicationLuceneDirectoryAsync() {
        var directory = _applicationContext.FileSystem
                                           .GetDatabaseDirectory()
                                           .GetDirectories(searchPattern: LuceneConstants.UniqueIndexName)
                                           .SingleOrDefault();

        if (directory is { IsEmpty: true }) {
            await NotifyInformationAsync().SkipContextSync();
        }

        return directory;
    }

    private IDirectory GetDestinationDirectory(string backupDestinationDirectoryPath) {
        return _applicationContext.DataDirectory
                                  .GetDirectory(
                                      Path.Combine(
                                          backupDestinationDirectoryPath,
                                          LuceneConstants.UniqueIndexName
                                      )
                                  );
    }
}
