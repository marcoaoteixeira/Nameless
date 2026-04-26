using System.Collections;
using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene.Collections;

public class SearchEnumerable : IEnumerable<ScoreDocument> {
    private readonly IndexSearcher _searcher;
    private readonly Query _query;
    private readonly Sort _sort;
    private readonly int _limit;

    public SearchEnumerable(IndexSearcher searcher, Query query, Sort sort, int limit) {
        _searcher = searcher;
        _query = query;
        _sort = sort;
        _limit = limit;
    }

    public IEnumerator<ScoreDocument> GetEnumerator() {
        return new SearchEnumerator(_searcher, _query, _sort, _limit);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}