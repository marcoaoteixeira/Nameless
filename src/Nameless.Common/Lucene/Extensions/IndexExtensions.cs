using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IIndex"/> extension methods.
/// </summary>
public static class IndexExtensions {
    /// <param name="self">
    ///     The current instance of <see cref="IIndex"/>.
    /// </param>
    extension(IIndex self) {
        /// <summary>
        ///     Retrieves the total number of documents for the index.
        /// </summary>
        /// <returns>
        ///     A <see cref="Result{T}"/> object where the type of the result
        ///     is an integer value representing the number of documents in
        ///     the index.
        /// </returns>
        public Result<int> Count() {
            return self.Count(new MatchAllDocsQuery());
        }

        /// <summary>
        ///     Searches the index using the specified query, using default relevance
        ///     sorting and the maximum query result limit.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/>.</returns>
        public IEnumerable<ScoreDocument> Search(Query query) {
            return self.Search(query, Sort.RELEVANCE, LuceneConstants.MaximumQueryResults);
        }

        /// <summary>
        ///     Searches the index using the specified query and sort, using the
        ///     maximum query result limit.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="sort">The sort options.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/>.</returns>
        public IEnumerable<ScoreDocument> Search(Query query, Sort sort) {
            return self.Search(query, sort, LuceneConstants.MaximumQueryResults);
        }

        /// <summary>
        ///     Searches the index using the specified query with a result limit,
        ///     using default relevance sorting.
        /// </summary>
        /// <param name="query">The search query.</param>
        /// <param name="limit">The maximum number of results to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/>.</returns>
        public IEnumerable<ScoreDocument> Search(Query query, int limit) {
            return self.Search(query, Sort.RELEVANCE, limit);
        }
    }
}
