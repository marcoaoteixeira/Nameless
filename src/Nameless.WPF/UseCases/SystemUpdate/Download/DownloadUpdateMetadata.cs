using System.Diagnostics.CodeAnalysis;

namespace Nameless.WPF.UseCases.SystemUpdate.Download;

public readonly record struct DownloadUpdateMetadata(string? FilePath) {
    [MemberNotNullWhen(returnValue: true, nameof(FilePath))]
    public bool IsUpdateAvailable => !string.IsNullOrWhiteSpace(FilePath);
}