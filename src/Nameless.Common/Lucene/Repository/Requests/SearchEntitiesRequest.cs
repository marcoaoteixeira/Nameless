namespace Nameless.Lucene.Repository.Requests;

/// <summary>
///     Represents a search request.
/// </summary>
/// <param name="IndexName">
///     The name of the index.
/// </param>
public record SearchEntitiesRequest(string? IndexName = null)
    : RequestBase(IndexName) {
    /// <summary>
    ///     Gets or sets the delegate that creates the
    ///     search query.
    /// </summary>
    public required Action<IQueryBuilder> Query { get; init; }
}