#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Search;

public sealed record IndexActionResult {
    /// <summary>
    /// Gets the total number of documents modified.
    /// </summary>
    public int Total { get; }

    /// <summary>
    /// Whether the action results in success or not.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
#endif
    public bool Succeeded => string.IsNullOrWhiteSpace(Error);

    /// <summary>
    /// Gets the error when not succeeded.
    /// </summary>
    public string? Error { get; }

    private IndexActionResult(int total = 0, string? error = null) {
        Total = total;
        Error = error;
    }

    public static IndexActionResult Success(int total)
        => new(total);

    public static IndexActionResult Failure(string error)
        => new(error: Prevent.Argument.NullOrWhiteSpace(error));
}