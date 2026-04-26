using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene;

public static class TopDocsExtensions {
    extension(TopDocs self) {
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