using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene.Internals;

internal static class TopFieldCollectorExtensions {
    extension(TopFieldCollector self) {
        internal DocumentCollection Collect(IndexSearcher searcher) {
            var documents = self.GetTopDocs()
                                .ScoreDocs
                                .Select(doc => new ScoreDocument(searcher.Doc(doc.Doc), doc.Score));

            return [.. documents];
        }
    }
}
