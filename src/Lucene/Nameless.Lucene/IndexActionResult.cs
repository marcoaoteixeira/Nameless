#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Lucene;

public sealed record IndexActionResult {
    public int Count { get; }

    public int Total { get; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
#endif
    public bool Succeeded
        => string.IsNullOrWhiteSpace(Error);
    
    public string Error { get; }

    private IndexActionResult(int count, int total, string error) {
        Count = count;
        Total = total;
        Error = error;
    }

    public static IndexActionResult Success(int count, int total)
        => new(count, total, error: string.Empty);

    public static IndexActionResult Failure(int count, int total, string error)
        => new(count: count, total, error: Prevent.Argument.NullOrWhiteSpace(error));
}