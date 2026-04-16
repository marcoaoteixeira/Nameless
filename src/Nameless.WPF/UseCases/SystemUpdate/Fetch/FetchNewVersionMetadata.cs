using System.Diagnostics.CodeAnalysis;

namespace Nameless.WPF.UseCases.SystemUpdate.Fetch;

public readonly record struct FetchNewVersionMetadata(string? Url) {
    [MemberNotNullWhen(returnValue: true, nameof(Url))]
    public bool IsNewVersionAvailable => !string.IsNullOrWhiteSpace(Url);
}