using Lucene.Net.Search;
using Nameless.Results;

namespace Nameless.Lucene.Infrastructure;

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
    }
}
