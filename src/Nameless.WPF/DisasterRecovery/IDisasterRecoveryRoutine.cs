namespace Nameless.WPF.DisasterRecovery;

public interface IDisasterRecoveryRoutine {
    string Name { get; }

    Task<BackupOutput> BackupAsync(BackupInput input, CancellationToken cancellationToken);

    Task<RestoreOutput> RestoreAsync(RestoreInput input, CancellationToken cancellationToken);
}
