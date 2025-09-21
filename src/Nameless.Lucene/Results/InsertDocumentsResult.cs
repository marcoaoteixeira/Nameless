namespace Nameless.Lucene.Results;

/// <summary>
///     Represents the result of an insert documents operation.
/// </summary>
public sealed record InsertDocumentsResult : Result {
    /// <summary>
    ///     Gets the number of documents that were successfully inserted.
    /// </summary>
    public int Count { get; }
    /// <summary>
    ///     Whether the operation was cancelled.
    /// </summary>
    public bool Cancelled { get; }

    private InsertDocumentsResult(int count, bool cancelled, string? error)
        : base(error) {
        Count = count;
        Cancelled = cancelled;
    }

    /// <summary>
    ///     Creates a successful <see cref="InsertDocumentsResult"/> instance.
    /// </summary>
    /// <param name="count">
    ///     The number of documents that were successfully inserted.
    /// </param>
    /// <param name="cancelled">
    ///     Whether the operation was cancelled.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="InsertDocumentsResult"/> representing
    ///     a successful operation.
    /// </returns>
    public static InsertDocumentsResult Success(int count, bool cancelled = false) {
        return new InsertDocumentsResult(count, cancelled, error: null);
    }

    /// <summary>
    ///     Creates a failure <see cref="InsertDocumentsResult"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error message.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="InsertDocumentsResult"/> representing
    ///     a failure operation.
    /// </returns>
    public static InsertDocumentsResult Failure(string error) {
        return new InsertDocumentsResult(
            count: 0,
            cancelled: false,
            error: Guard.Against.NullOrWhiteSpace(error)
        );
    }
}