using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Windows.UseCases.Backup;

public sealed class PerformApplicationBackupResponse : Result<PerformApplicationBackupMetadata> {
    private PerformApplicationBackupResponse(PerformApplicationBackupMetadata value, Error[] errors)
        : base(value, errors) { }

    public static implicit operator PerformApplicationBackupResponse(PerformApplicationBackupMetadata value) {
        return new PerformApplicationBackupResponse(value, errors: []);
    }

    public static implicit operator PerformApplicationBackupResponse(Error[] errors) {
        return new PerformApplicationBackupResponse(value: default, errors);
    }

    public static implicit operator PerformApplicationBackupResponse(Error error) {
        return new PerformApplicationBackupResponse(value: default, errors: [error]);
    }
}