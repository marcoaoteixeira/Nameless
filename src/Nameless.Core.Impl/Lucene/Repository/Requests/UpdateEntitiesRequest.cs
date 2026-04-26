namespace Nameless.Lucene.Repository.Requests;

/// <summary>
///     Represents a request to update entities.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the entity.
/// </typeparam>
/// <param name="Entities">
///     The entities.
/// </param>
/// <param name="IndexName">
///     The index name.
/// </param>
public record UpdateEntitiesRequest<TEntity>(TEntity[] Entities, string? IndexName = null)
    : RequestBase(IndexName) where TEntity : class;