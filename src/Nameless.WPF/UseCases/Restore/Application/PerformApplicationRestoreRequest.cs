using System.IO;
using Nameless.Application;
using Nameless.Compression;
using Nameless.Compression.Requests;
using Nameless.Mediator.Requests;
using Nameless.ObjectModel;
using Nameless.Results;
using Nameless.WPF.DisasterRecovery;
using Nameless.WPF.Messaging;

namespace Nameless.WPF.UseCases.Restore.Application;

public class PerformApplicationRestoreResponse : Result<bool> {
    private PerformApplicationRestoreResponse(bool value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator PerformApplicationRestoreResponse(bool value) {
        return new PerformApplicationRestoreResponse(value, errors: []);
    }

    public static implicit operator PerformApplicationRestoreResponse(Error error) {
        return new PerformApplicationRestoreResponse(value: false, errors: [error]);
    }

    public static implicit operator PerformApplicationRestoreResponse(Error[] errors) {
        return new PerformApplicationRestoreResponse(value: false, errors);
    }
}

public class PerformApplicationRestoreRequest : IRequest<PerformApplicationRestoreResponse> {
    public required DateTimeOffset Timestamp { get; init; }
}

public class PerformApplicationRestoreRequestHandler : IRequestHandler<PerformApplicationRestoreRequest, PerformApplicationRestoreResponse> {
    private readonly IApplicationContext _applicationContext;
    private readonly IDisasterRecoveryRoutine[] _disasterRecoveryRoutines;
    private readonly ICompressor _compressor;

    public PerformApplicationRestoreRequestHandler(
        IApplicationContext applicationContext,
        ICompressor compressor,
        IEnumerable<IDisasterRecoveryRoutine> disasterRecoveryRoutines) {
        _applicationContext = applicationContext;
        _compressor = compressor;
        _disasterRecoveryRoutines = [.. disasterRecoveryRoutines];
    }

    public async Task<PerformApplicationRestoreResponse> HandleAsync(PerformApplicationRestoreRequest request, CancellationToken cancellationToken) {
        var backupFilePath = GetBackupFilePath(request.Timestamp);
        if (backupFilePath.Failure) { return backupFilePath.Errors; }

        var backupDirectoryPath = await DecompressBackupFileAsync(
            backupFilePath.Value,
            cancellationToken
        ).SkipContextSync();
        if (backupDirectoryPath.Failure) { return backupDirectoryPath.Errors; }

        // execute the restore routines
        var input = new RestoreInput {
            TemporaryDirectoryPath = backupDirectoryPath.Value
        };
        var routines = _disasterRecoveryRoutines.Select(
            routine => routine.RestoreAsync(input, cancellationToken)
        );

        var results = await Task.WhenAll(routines).SkipContextSync();

        foreach (var result in results) {
            if (result.Success) { continue; }

            // notify of possible errors.
        }

        throw new NotImplementedException();
    }

    private Result<string> GetBackupFilePath(DateTimeOffset timestamp) {
        // backup files should always be in the "backups" directory.
        var backupFileName = string.Format(WPFConstants.BackupFileNamePattern, timestamp);
        var backupDirectory = _applicationContext.FileSystemProvider.GetBackupDirectory();
        var backupFile = _applicationContext.FileSystemProvider.GetFile(
            Path.Combine(backupDirectory.Path, backupFileName)
        );

        if (!backupFile.Exists) {
            return Error.Missing("Backup file not found");
        }

        return backupFile.Path;
    }

    private async Task<Result<string>> DecompressBackupFileAsync(string backupFilePath, CancellationToken cancellationToken) {
        // decompress file into the temporary directory
        var request = new DecompressRequest(backupFilePath) {
            DestinationDirectoryPath = _applicationContext.FileSystemProvider.GetTemporaryDirectory().Path
        };

        var response = await _compressor.DecompressAsync(request, cancellationToken)
                                                .SkipContextSync();

        return response.Match<Result<string>>(
            onSuccess: value => value.DirectoryPath,
            onFailure: errors => errors
        );
    }
}

public record PerformApplicationRestoreMessage : Message;