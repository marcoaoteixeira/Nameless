using Lucene.Net.Search;

namespace Nameless.Lucene.Internals;

/// <summary>
///     <see cref="ScoreDoc"/> extension methods.
/// </summary>
internal static class ScoreDocExtensions {
    /// <summary>
    ///     Converts a <see cref="ScoreDoc"/> to an <see cref="ISearchHit"/>.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ScoreDoc"/>.
    /// </param>
    /// <param name="indexSearcher">
    ///     The <see cref="IndexSearcher"/> used to retrieve the document.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="ISearchHit"/>.
    /// </returns>
    internal static ISearchHit ToSearchHit(this ScoreDoc self, IndexSearcher indexSearcher) {
        var document = indexSearcher.Doc(self.Doc);

        return new SearchHit(document, self.Score);
    }
}