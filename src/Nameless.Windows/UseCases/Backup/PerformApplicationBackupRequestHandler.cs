using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Application;
using Nameless.Compression;
using Nameless.Compression.Requests;
using Nameless.IO.FileSystem;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.Windows.DisasterRecovery;
using Nameless.Windows.Messaging;
using Nameless.Windows.Resources;

namespace Nameless.Windows.UseCases.Backup;

public class PerformApplicationBackupRequestHandler : RequestHandlerBase<PerformApplicationBackupRequestHandler, PerformApplicationBackupRequest, PerformApplicationBackupResponse> {
    private readonly IApplicationContext _applicationContext;
    private readonly ICompressor _compressor;
    private readonly IDisasterRecoveryRoutine[] _routines;
    private readonly TimeProvider _timeProvider;
    
    public PerformApplicationBackupRequestHandler(
        IApplicationContext applicationContext,
        ICompressor compressor,
        IEnumerable<IDisasterRecoveryRoutine> disasterRecoveryRoutines,
        IMessenger messenger,
        TimeProvider timeProvider
        ) : base(messenger, NullLogger<PerformApplicationBackupRequestHandler>.Instance) {
        _applicationContext = applicationContext;
        _compressor = compressor;
        _routines = [.. disasterRecoveryRoutines];
        _timeProvider = timeProvider;
    }

    public override async Task<PerformApplicationBackupResponse> HandleAsync(PerformApplicationBackupRequest request, CancellationToken cancellationToken) {
        await NotifyInformationAsync(
            Strings.PerformApplicationBackup_Event_Message_Starting
        ).SkipContextSync();
        
        var temporaryDirectory = CreateTemporaryDirectory(
            timestamp: _timeProvider.GetUtcNow()
        );

        // backup routine is all or nothing.
        var backupRoutinesResult = await ExecuteBackupRoutinesAsync(
            temporaryDirectory,
            cancellationToken
        ).SkipContextSync();

        if (backupRoutinesResult.Failure) {
            return backupRoutinesResult.Errors;
        }

        // compress everything from backup
        var compressBackupFilesResult = await CompressBackupFilesAsync(
            temporaryDirectory,
            cancellationToken
        );

        if (compressBackupFilesResult.Failure) {
            return compressBackupFilesResult.Errors;
        }

        await CleanUpAsync(temporaryDirectory).SkipContextSync();

        await NotifySuccessAsync(
            string.Format(Strings.PerformApplicationBackup_Event_Message_Success, compressBackupFilesResult.Value)
        ).SkipContextSync();

        return compressBackupFilesResult.Match<PerformApplicationBackupResponse>(
            onSuccess: value => new PerformApplicationBackupMetadata(value),
            onFailure: errors => errors
        );
    }

    private IDirectory CreateTemporaryDirectory(DateTimeOffset timestamp) {
        var result = _applicationContext.FileSystemProvider.GetTemporaryDirectory(
            timestamp
        );

        result.Create(); // Ensure directory existence.

        return result;
    }

    private async Task<Result<string[]>> ExecuteBackupRoutinesAsync(IDirectory temporaryDirectory, CancellationToken cancellationToken) {
        var input = new BackupInput {
            SourceDirectoryPath = FolderStructure.DatabaseDirectoryName,
            DestinationDirectoryPath = temporaryDirectory.Path
        };

        var routines = _routines.Select(
            routine => routine.BackupAsync(input, cancellationToken)
        ).ToArray();

        var results = await Task.WhenAll(routines).SkipContextSync();

        var fileCollection = new List<string>();
        var errorCollection = new List<Error>();

        foreach (var result in results) {
            result.Match(
                onSuccess: fileCollection.AddRange,
                onFailure: errorCollection.AddRange
            );
        }

        if (errorCollection.Count <= 0) {
            return fileCollection.ToArray();
        }

        await NotifyFailureAsync(
            Strings.PerformApplicationBackup_Event_Message_Failure
        ).SkipContextSync();

        // if an error occur, drop all backup files
        await CleanUpAsync(temporaryDirectory).SkipContextSync();

        return errorCollection.ToArray();
    }

    private async Task<Result<string>> CompressBackupFilesAsync(IDirectory temporaryDirectory, CancellationToken cancellationToken) {
        _applicationContext.FileSystemProvider.GetBackupDirectory().Create(); // ensure existence

        // create a file that represents the temporary directory.
        var backupFileName = $"{temporaryDirectory.Name}{WindowsConstants.BackupFileExtension}";
        var backupDirectory = _applicationContext.FileSystemProvider.GetBackupDirectory();
        var backupFile = _applicationContext.FileSystemProvider.GetFile(
            Path.Combine(backupDirectory.Path, backupFileName)
        );
        
        var request = new CompressRequest(backupFile.Path);

        foreach (var file in temporaryDirectory.GetFiles(recursive: true)) {
            var directory = Path.GetDirectoryName(
                Path.GetRelativePath(temporaryDirectory.Path, file.Path)
            );

            request.IncludeFile(file.Path, directory);
        }

        var response = await _compressor.CompressAsync(
            request,
            cancellationToken
        ).SkipContextSync();

        if (response.Success) {
            return response.Value.FilePath;
        }

        await NotifyFailureAsync(
            Strings.PerformApplicationBackup_Event_Message_Failure
        ).SkipContextSync();

        // if an error occur, drop all backup files
        await CleanUpAsync(temporaryDirectory).SkipContextSync();

        return response.Errors;
    }

    private async Task CleanUpAsync(IDirectory outputDirectory) {
        await NotifyInformationAsync(
            Strings.PerformApplicationBackup_Event_Message_CleanUp
        ).SkipContextSync();

        // cleanup the temporary directory
        outputDirectory.Delete(recursive: true);
    }
}