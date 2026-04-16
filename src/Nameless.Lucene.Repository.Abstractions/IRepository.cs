using Nameless.Lucene.Repository.Requests;
using Nameless.Lucene.Repository.Responses;

namespace Nameless.Lucene.Repository;

/// <summary>
///     Represents a repository for Lucene indexes.
/// </summary>
public interface IRepository {
    /// <summary>
    ///     Inserts the specified request documents into the Lucene index.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> represeting the asynchronous
    ///     execution, where the result is an
    ///     <see cref="InsertEntitiesResponse"/>.
    /// </returns>
    Task<InsertEntitiesResponse> InsertAsync<TEntity>(InsertEntitiesRequest<TEntity> request, CancellationToken cancellationToken)
        where TEntity : class;

    /// <summary>
    ///     Deletes the specified request documents from the Lucene index.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> represeting the asynchronous
    ///     execution, where the result is a
    ///     <see cref="DeleteEntitiesResponse"/>.
    /// </returns>
    Task<DeleteEntitiesResponse> DeleteAsync<TEntity>(DeleteEntitiesRequest<TEntity> request, CancellationToken cancellationToken)
            where TEntity : class;

    /// <summary>
    ///     Deletes the query resulting documents from the Lucene index.
    /// </summary>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> represeting the asynchronous
    ///     execution, where the result is a
    ///     <see cref="DeleteEntitiesByQueryResponse"/>.
    /// </returns>
    Task<DeleteEntitiesByQueryResponse> DeleteByQueryAsync(DeleteEntitiesByQueryRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Updates the specified documents in the Lucene index.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> represeting the asynchronous
    ///     execution, where the result is an
    ///     <see cref="UpdateEntitiesResponse"/>.
    /// </returns>
    Task<UpdateEntitiesResponse> UpdateAsync<TEntity>(UpdateEntitiesRequest<TEntity> request, CancellationToken cancellationToken)
        where TEntity : class;

    /// <summary>
    ///     Searches documents in a Lucene index.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}"/> represeting the asynchronous
    ///     enumerable that contains the search result.
    /// </returns>
    IAsyncEnumerable<TEntity> SearchAsync<TEntity>(SearchEntitiesRequest request)
        where TEntity : class, new();
}