using System.Diagnostics.CodeAnalysis;

namespace Nameless.WPF.UseCases.SystemUpdate.Check;

public readonly record struct CheckForUpdateMetadata(
    int ReleaseID,
    string? ApplicationName,
    string? Version
) {
    [MemberNotNullWhen(returnValue: true, nameof(ApplicationName), nameof(Version))]
    public bool IsNewVersionAvailable => ReleaseID > 0;
}