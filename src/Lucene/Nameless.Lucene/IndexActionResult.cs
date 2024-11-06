#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Lucene;

public sealed record IndexActionResult {
    public int Count { get; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
#endif
    public bool Succeeded
        => string.IsNullOrWhiteSpace(Error);
    
    public string Error { get; }

    private IndexActionResult(int count, string error) {
        Count = count;
        Error = error;
    }

    public static IndexActionResult Success(int count)
        => new(count, error: string.Empty);

    public static IndexActionResult Failure(int count, string error)
        => new(count: count, error: Prevent.Argument.NullOrWhiteSpace(error));
}