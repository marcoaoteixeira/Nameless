using System.Diagnostics.CodeAnalysis;

namespace Nameless.Search;

public sealed record IndexActionResult {
    /// <summary>
    ///     Gets the total number of documents modified.
    /// </summary>
    public int Total { get; }

    /// <summary>
    ///     Whether the action results in success or not.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => string.IsNullOrWhiteSpace(Error);

    /// <summary>
    ///     Gets the error when not succeeded.
    /// </summary>
    public string? Error { get; }

    private IndexActionResult(int total = 0, string? error = null) {
        Total = total;
        Error = error;
    }

    public static IndexActionResult Success(int total) {
        return new IndexActionResult(total);
    }

    public static IndexActionResult Failure(string error) {
        return new IndexActionResult(error: Prevent.Argument.NullOrWhiteSpace(error));
    }
}