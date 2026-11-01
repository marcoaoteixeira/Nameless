using System.Collections;
using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene.Collections;

/// <summary>
///     A lazy, enumerable wrapper around a Lucene search query that creates a
///     <see cref="SearchEnumerator"/> on each iteration.
/// </summary>
public class SearchEnumerable : IEnumerable<ScoreDocument> {
    private readonly IndexSearcher _searcher;
    private readonly Query _query;
    private readonly Sort _sort;
    private readonly int _limit;

    /// <summary>
    ///     Initializes a new <see cref="SearchEnumerable"/> with the given search parameters.
    /// </summary>
    /// <param name="searcher">The <see cref="IndexSearcher"/> used to execute the query.</param>
    /// <param name="query">The Lucene <see cref="Query"/> to execute.</param>
    /// <param name="sort">The <see cref="Sort"/> order for results.</param>
    /// <param name="limit">The maximum number of documents to return per page.</param>
    public SearchEnumerable(IndexSearcher searcher, Query query, Sort sort, int limit) {
        _searcher = searcher;
        _query = query;
        _sort = sort;
        _limit = limit;
    }

    /// <inheritdoc />
    public IEnumerator<ScoreDocument> GetEnumerator() {
        return new SearchEnumerator(_searcher, _query, _sort, _limit);
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}