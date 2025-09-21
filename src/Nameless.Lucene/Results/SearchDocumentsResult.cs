namespace Nameless.Lucene.Results;

/// <summary>
///     Represents the result of a document search operation.
/// </summary>
public sealed record SearchDocumentsResult : Result {
    /// <summary>
    ///     Gets the total number of documents that match the search criteria.
    /// </summary>
    public int Count { get; }

    /// <summary>
    ///     Gets an array of search hits that match the search criteria.
    /// </summary>
    public ISearchHit[] Results { get; } = [];

    private SearchDocumentsResult(int count, ISearchHit[] results, string? error)
        : base(error) {
        Count = count;
        Results = results;
    }

    /// <summary>
    ///     Creates a successful <see cref="SearchDocumentsResult"/> instance.
    /// </summary>
    /// <param name="count">
    ///     The total number of documents that match the search criteria.
    /// </param>
    /// <param name="results">
    ///     The array of search hits that match the search criteria.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="SearchDocumentsResult"/> representing
    ///     a successful operation.
    /// </returns>
    public static SearchDocumentsResult Success(int count, ISearchHit[] results) {
        return new SearchDocumentsResult(count, results, error: null);
    }

    /// <summary>
    ///     Creates a failure <see cref="SearchDocumentsResult"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error message.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="SearchDocumentsResult"/> representing
    ///     a failure operation.
    /// </returns>
    public static SearchDocumentsResult Failure(string error) {
        return new SearchDocumentsResult(
            count: 0,
            results: [],
            error: Guard.Against.NullOrWhiteSpace(error)
        );
    }
}