using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Requests;
using Nameless.Lucene.Responses;

namespace Nameless.Lucene;

/// <summary>
///     Represents a repository for Lucene indexes.
/// </summary>
public interface IRepository {
    /// <summary>
    ///     Creates a query builder for the specified index.
    /// </summary>
    /// <param name="indexName">
    ///     The index name.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IQueryBuilder"/>.
    /// </returns>
    IQueryBuilder CreateQueryBuilder(string? indexName);

    /// <summary>
    ///     Inserts the specified request documents into the Lucene index.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     Type of the document.
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
    ///     <see cref="InsertDocumentsResponse"/>.
    /// </returns>
    Task<InsertDocumentsResponse> InsertAsync<TDocument>(InsertDocumentsRequest<TDocument> request, CancellationToken cancellationToken)
        where TDocument : class;

    /// <summary>
    ///     Deletes the specified request documents from the Lucene index.
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
    ///     <see cref="DeleteDocumentsResponse"/>.
    /// </returns>
    Task<DeleteDocumentsResponse> DeleteAsync(DeleteDocumentsRequest request, CancellationToken cancellationToken);

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
    ///     <see cref="DeleteDocumentsByQueryResponse"/>.
    /// </returns>
    Task<DeleteDocumentsByQueryResponse> DeleteByQueryAsync(DeleteDocumentsByQueryRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Searches documents in a Lucene index.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     Type of the document.
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
    ///     <see cref="SearchDocumentsResponse{TDocument}"/>.
    /// </returns>
    Task<SearchDocumentsResponse<TDocument>> SearchAsync<TDocument>(SearchDocumentsRequest request, CancellationToken cancellationToken)
        where TDocument : class, new();

    /// <summary>
    ///     Streams a collection of documents from a Lucene index, given the request query.
    /// </summary>
    /// <typeparam name="TDocument">
    ///     Type of the document.
    /// </typeparam>
    /// <param name="request">
    ///     The request.
    /// </param>
    /// <returns>
    ///     An <see cref="IAsyncEnumerable{T}"/> represeting the asynchronous
    ///     stream of <typeparamref name="TDocument"/>.
    /// </returns>
    IAsyncEnumerable<TDocument> StreamAsync<TDocument>(SearchDocumentsRequest request)
        where TDocument : class, new();
}