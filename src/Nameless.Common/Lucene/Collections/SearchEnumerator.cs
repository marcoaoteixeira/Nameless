using System.Collections;
using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene.Collections;

/// <summary>
///     Enumerates <see cref="ScoreDocument"/> results from a Lucene search, fetching
///     documents in pages using search-after pagination.
/// </summary>
public sealed class SearchEnumerator : IEnumerator<ScoreDocument> {
    private readonly IndexSearcher _searcher;
    private readonly Query _query;
    private readonly Sort _sort;
    private readonly int _limit;

    private ScoreDoc? _after;
    private ScoreDocument? _current;
    private Queue<ScoreDocument> _documents = [];

    private bool _disposed;

    /// <inheritdoc />
    public ScoreDocument Current => GetCurrent();

    object IEnumerator.Current => GetCurrent();

    /// <summary>
    ///     Initializes a new <see cref="SearchEnumerator"/> with the given search parameters.
    /// </summary>
    /// <param name="searcher">The <see cref="IndexSearcher"/> used to execute searches.</param>
    /// <param name="query">The Lucene <see cref="Query"/> to execute.</param>
    /// <param name="sort">The <see cref="Sort"/> order for results.</param>
    /// <param name="limit">
    ///     The maximum number of documents per page; must be between 1 and
    ///     <see cref="LuceneConstants.MaximumQueryResults"/>.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     if <paramref name="limit"/> is out of the valid range.
    /// </exception>
    public SearchEnumerator(IndexSearcher searcher, Query query, Sort sort, int limit) {
        _searcher = searcher;
        _query = query;
        _sort = sort;
        _limit = Throws.When.OutOfRange(
            paramValue: limit,
            minimumValue: 1,
            maximumValue: LuceneConstants.MaximumQueryResults
        );
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~SearchEnumerator() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public bool MoveNext() {
        BlockAccessAfterDispose();

        if (!_documents.TryDequeue(out _current)) {
            _current = Fetch();
        }

        return _current is not null;
    }

    /// <inheritdoc />
    public void Reset() {
        BlockAccessAfterDispose();

        _after = null;
        _current = null;
        _documents.Clear();
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            /* dispose managed resources */
        }

        _after = null;
        _current = null;
        _documents.Clear();

        _disposed = true;
    }

    private ScoreDocument? Fetch() {
        // executes the search
        var hits = _searcher.SearchAfter(_after, _query, _limit, _sort);

        // get the last ScoreDoc, it will be used to paginate for
        // the next interaction.
        _after = hits.ScoreDocs.LastOrDefault();
        
        // collect the documents and create queue
        _documents = hits.Collect(_searcher)
                         .CreateQueue();

        // this will ensure that if we have a successful query
        // we return a document or stop the enumerator.
        return _documents.TryDequeue(out var result) ? result : null;
    }

    private ScoreDocument GetCurrent() {
        BlockAccessAfterDispose();

        return _current ?? throw new InvalidOperationException("Enumerator was not initialized.");
    }
}