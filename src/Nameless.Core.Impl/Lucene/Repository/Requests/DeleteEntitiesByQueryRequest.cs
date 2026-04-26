namespace Nameless.Lucene.Repository.Requests;

/// <summary>
///     Represents a request to delete entities.
/// </summary>
/// <param name="IndexName">
///     The index name.
/// </param>
public record DeleteEntitiesByQueryRequest(string? IndexName = null)
    : RequestBase(IndexName) {
    /// <summary>
    ///     Gets or sets the delegate responsible for
    ///     creating the delete query.
    /// </summary>
    public required Action<IQueryBuilder> Query { get; init; }
}