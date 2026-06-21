using System.Diagnostics.CodeAnalysis;

namespace Nameless.Windows.UseCases.Backup;

public readonly record struct PerformApplicationBackupMetadata(string? BackupPath) {
    [MemberNotNullWhen(returnValue: false, nameof(BackupPath))]
    public bool Skip => string.IsNullOrWhiteSpace(BackupPath);
}