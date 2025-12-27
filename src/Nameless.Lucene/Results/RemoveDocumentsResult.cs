namespace Nameless.Lucene.Results;

/// <summary>
///     Represents the result of a remove documents operation.
/// </summary>
public sealed record RemoveDocumentsResult : Result {
    /// <summary>
    ///     Gets the number of documents that were successfully removed.
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     Whether the operation was cancelled.
    /// </summary>
    public bool Cancelled { get; }

    private RemoveDocumentsResult(int count, bool cancelled, string? error)
        : base(error) {
        Count = count;
        Cancelled = cancelled;
    }

    /// <summary>
    ///     Creates a successful <see cref="RemoveDocumentsResult"/> instance.
    /// </summary>
    /// <param name="count">
    ///     The number of documents that were successfully removed.
    /// </param>
    /// <param name="cancelled">
    ///     Whether the operation was cancelled.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="RemoveDocumentsResult"/> representing
    ///     a successful operation.
    /// </returns>
    public static RemoveDocumentsResult Success(int count, bool cancelled = false) {
        return new RemoveDocumentsResult(count, cancelled, error: null);
    }

    /// <summary>
    ///     Creates a failure <see cref="RemoveDocumentsResult"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error message.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="RemoveDocumentsResult"/> representing
    ///     a failure operation.
    /// </returns>
    public static RemoveDocumentsResult Failure(string error) {
        return new RemoveDocumentsResult(
            count: 0,
            cancelled: false,
            Guard.Against.NullOrWhiteSpace(error)
        );
    }
}