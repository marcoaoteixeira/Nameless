namespace Nameless.Lucene.Repository.Requests;

/// <summary>
///     Represents a request to delete entities.
/// </summary>
/// <param name="Entities">
///     The entities.
/// </param>
/// <param name="IndexName">
///     The index name.
/// </param>
public record DeleteEntitiesRequest<TEntity>(TEntity[] Entities, string? IndexName = null)
    : RequestBase(IndexName) where TEntity : class;