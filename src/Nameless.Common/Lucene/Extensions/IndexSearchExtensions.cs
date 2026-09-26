using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="TopDocs"/> extension methods.
/// </summary>
public static class TopDocsExtensions {
    /// <param name="self">The current <see cref="TopDocs"/> instance.</param>
    extension(TopDocs self) {
        /// <summary>
        ///     Collects documents from the search results by fetching each
        ///     document from the searcher.
        /// </summary>
        /// <param name="searcher">The index searcher used to load documents.</param>
        /// <returns>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="ScoreDocument"/>.
        /// </returns>
        public IEnumerable<ScoreDocument> Collect(IndexSearcher searcher) {
            return self.ScoreDocs.Select(
                hit => new ScoreDocument(
                    searcher.Doc(hit.Doc),
                    hit.Score
                )
            );
        }
    }
}