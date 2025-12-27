using Lucene.Net.Search;
using Nameless.Lucene.Results;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IIndex"/> extension methods.
/// </summary>
public static class IndexExtensions {
    /// <summary>
    ///     Executes a search asynchronously against the index.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IIndex"/>.
    /// </param>
    /// <param name="query">
    ///     The query.
    /// </param>
    /// <param name="sort">
    ///     The sort. Defaults to <see cref="Defaults.Sort"/>, if not
    ///     specified.
    /// </param>
    /// <param name="start">
    ///     The start document. Defaults to <c>0</c>, if not specified.
    /// </param>
    /// <param name="limit">
    ///     The maximum number of documents to return. Defaults to
    ///     <see cref="Defaults.QUERY_MAXIMUM_RESULTS"/>, if not specified.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> representing the asynchronous
    ///     operation, where the result contains an instance of
    ///     <see cref="SearchDocumentsResult" />.
    /// </returns>
    public static Task<SearchDocumentsResult> SearchAsync(this IIndex self, Query query, Sort? sort = null,
        int start = 0, int limit = Defaults.QUERY_MAXIMUM_RESULTS, CancellationToken cancellationToken = default) {
        return Guard.Against
                    .Null(self)
                    .SearchAsync(query, sort ?? Defaults.Sort, start, limit, cancellationToken);
    }
}