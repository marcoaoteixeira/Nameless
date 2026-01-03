using Nameless.Lucene.Requests;
using Nameless.Lucene.Responses;

namespace Nameless.Lucene;

/// <summary>
///     Defines an index.
/// </summary>
public interface IIndex : IDisposable {
    /// <summary>
    ///     Creates a new instance of <see cref="IQueryBuilder" />.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="IQueryBuilder" />.
    /// </returns>
    IQueryBuilder CreateQueryBuilder();

    /// <summary>
    ///     Inserts the documents asynchronously into the index.
    /// </summary>
    /// <param name="request">
    ///     The insert document request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="InsertDocumentsResponse" />.
    /// </returns>
    Task<InsertDocumentsResponse> InsertDocumentsAsync(InsertDocumentsRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes the documents asynchronously from the index.
    /// </summary>
    /// <param name="request">
    ///     The delete document request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="DeleteDocumentsResponse" />.
    /// </returns>
    Task<DeleteDocumentsResponse> DeleteDocumentsAsync(DeleteDocumentsRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes documents asynchronously from the index given the
    ///     results obtained from a query.
    /// </summary>
    /// <param name="request">
    ///     The delete by query request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="DeleteDocumentsByQueryResponse" />.
    /// </returns>
    Task<DeleteDocumentsByQueryResponse> DeleteDocumentsAsync(DeleteDocumentsByQueryRequest request, CancellationToken cancellationToken);

    /// <summary>
    ///     Executes a search asynchronously against the index.
    /// </summary>
    /// <param name="request">
    ///     The search request.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="SearchDocumentsResponse" />.
    /// </returns>
    Task<SearchDocumentsResponse> SearchDocumentsAsync(SearchDocumentsRequest request, CancellationToken cancellationToken);
}