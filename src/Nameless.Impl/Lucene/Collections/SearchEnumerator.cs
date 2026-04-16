using System.Collections;
using Lucene.Net.Search;
using Nameless.Lucene.ObjectModel;

namespace Nameless.Lucene.Collections;

public sealed class SearchEnumerator : IEnumerator<ScoreDocument> {
    private readonly IndexSearcher _searcher;
    private readonly Query _query;
    private readonly Sort _sort;
    private readonly int _limit;

    private ScoreDoc? _after;
    private ScoreDocument? _current;
    private Queue<ScoreDocument> _documents = [];

    private bool _disposed;

    public ScoreDocument Current => GetCurrent();

    object IEnumerator.Current => GetCurrent();

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

    ~SearchEnumerator() {
        Dispose(disposing: false);
    }

    public bool MoveNext() {
        BlockAccessAfterDispose();

        if (!_documents.TryDequeue(out _current)) {
            _current = Fetch();
        }

        return _current is not null;
    }

    public void Reset() {
        BlockAccessAfterDispose();

        _after = null;
        _current = null;
        _documents.Clear();
    }

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