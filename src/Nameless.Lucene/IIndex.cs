using Lucene.Net.Search;
using Nameless.Lucene.Results;

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
    /// <param name="documents">
    ///     The documents.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="InsertDocumentsResult" />.
    /// </returns>
    Task<InsertDocumentsResult> InsertAsync(IDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes the documents asynchronously from the index.
    /// </summary>
    /// <param name="documents">
    ///     The documents.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="RemoveDocumentsResult" />.
    /// </returns>
    Task<RemoveDocumentsResult> RemoveAsync(IDocument[] documents, CancellationToken cancellationToken);

    /// <summary>
    ///     Removes documents asynchronously from the index given the
    ///     results obtained from a query.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="RemoveDocumentsResult" />.
    /// </returns>
    Task<RemoveDocumentsResult> RemoveAsync(Query query, CancellationToken cancellationToken);

    /// <summary>
    ///     Executes a search asynchronously against the index.
    /// </summary>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <param name="sort">
    ///     The sort.
    /// </param>
    /// <param name="start">
    ///     The start document.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of documents to return.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="SearchDocumentsResult" />.
    /// </returns>
    Task<SearchDocumentsResult> SearchAsync(Query query, Sort sort, int start, int limit, CancellationToken cancellationToken);
}