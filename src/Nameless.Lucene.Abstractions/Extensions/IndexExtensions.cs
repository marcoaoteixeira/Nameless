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

        public IEnumerable<ScoreDocument> Search(Query query) {
            return self.Search(query, Sort.RELEVANCE, Constants.MAXIMUM_QUERY_RESULTS);
        }

        public IEnumerable<ScoreDocument> Search(Query query, Sort sort) {
            return self.Search(query, sort, Constants.MAXIMUM_QUERY_RESULTS);
        }

        public IEnumerable<ScoreDocument> Search(Query query, int limit) {
            return self.Search(query, Sort.RELEVANCE, limit);
        }
    }
}
