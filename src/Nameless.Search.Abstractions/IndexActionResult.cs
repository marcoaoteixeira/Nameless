using System.Diagnostics.CodeAnalysis;

namespace Nameless.Search;

public sealed record IndexActionResult {
    /// <summary>
    ///     Gets the totalDocumentsAffected number of documents affected.
    /// </summary>
    public int TotalDocumentsAffected { get; }

    /// <summary>
    ///     Whether the action results was successful or not.
    /// </summary>
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
    public bool Succeeded => string.IsNullOrWhiteSpace(Error);

    /// <summary>
    ///     Gets the error message when not succeeded.
    /// </summary>
    public string? Error { get; }

    private IndexActionResult(int totalDocumentsAffected = 0, string? error = null) {
        TotalDocumentsAffected = Prevent.Argument.LowerThan(totalDocumentsAffected, minValue: 0);
        Error = error;
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <param name="totalDocumentsAffected">The number of documents affected.</param>
    /// <returns>
    /// An instance of <see cref="IndexActionResult"/> for success.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If <paramref name="totalDocumentsAffected" /> is lower than 0.
    /// </exception>
    public static IndexActionResult Success(int totalDocumentsAffected) {
        return new IndexActionResult(totalDocumentsAffected, error: null);
    }

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>
    /// An instance of <see cref="IndexActionResult"/> for success.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If <paramref name="error"/> is empty or white spaces.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="error"/> is <see langword="null"/>.
    /// </exception>
    public static IndexActionResult Failure(string error) {
        return new IndexActionResult(totalDocumentsAffected: 0, Prevent.Argument.NullOrWhiteSpace(error));
    }
}