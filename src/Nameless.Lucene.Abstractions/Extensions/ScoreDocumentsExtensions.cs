using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene;

public static class ScoreDocumentsExtensions {
    extension(IEnumerable<ScoreDocument> self) {
        public Queue<ScoreDocument> CreateQueue() {
            return new Queue<ScoreDocument>(self);
        }
    }
}