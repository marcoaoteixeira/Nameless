using Lucene.Net.Search;

namespace Nameless.Lucene.Internals;

internal static class Collectors {
    internal static TopFieldCollector CreateTopCollector(int start, int limit, Sort sort) {
        return TopFieldCollector.Create(
            sort,
            start + limit,
            fillFields: false,
            trackDocScores: true,
            trackMaxScore: false,
            docsScoredInOrder: true
        );
    }
}
