using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Internals;
using Nameless.Lucene.Mapping;
using Nameless.Lucene.ObjectModel;
using Nameless.Lucene.Requests;
using Nameless.ObjectModel;

namespace Nameless.Lucene.Collections;

public class StreamAsyncEnumerator<TDocument> : IAsyncEnumerator<TDocument>
    where TDocument : class, new() {
    private readonly EntityDocumentBuilder _entityDocumentBuilder;
    private readonly IIndex _index;
    private readonly SearchDocumentsRequest _request;
    private readonly CancellationToken _cancellationToken;

    private Queue<TDocument> _documents = [];
    private TDocument? _current;
    private int _from;
    private int? _totalCount;

    private bool _disposed;

    public StreamAsyncEnumerator(EntityDocumentBuilder entityDocumentBuilder, IIndex index, SearchDocumentsRequest request, CancellationToken cancellationToken) {
        _entityDocumentBuilder = entityDocumentBuilder;
        _index = index;
        _request = request;
        _cancellationToken = cancellationToken;
    }

    public TDocument Current {
        get => _current ?? throw new NullReferenceException("Async enumerator not initialized.");
    }

    public ValueTask<bool> MoveNextAsync() {
        BlockAccessAfterDispose();

        _cancellationToken.ThrowIfCancellationRequested();

        if (!_documents.TryDequeue(out _current)) {
            Fetch();
        }

        return ValueTask.FromResult(
            _current is not null
        );
    }

    public ValueTask DisposeAsync() {
        if (_disposed) {
            return ValueTask.CompletedTask;
        }

        _documents.Clear();
        _current = null;

        GC.SuppressFinalize(this);

        _disposed = true;

        return ValueTask.CompletedTask;
    }

    private void Fetch() {
        if (_from >= _totalCount) { return; } // reach the search limit

        var request = _request with { Start = _from };

        var collector = Collectors.CreateTopCollector(
            request.Start,
            request.Limit,
            request.Sort
        );
        var search = _index.Search(request.Query, collector);

        search.Match(OnSearchSuccess, OnSearchFailure);
    }

    private void OnSearchSuccess(DocumentCollection collection) {
        _documents = new Queue<TDocument>(
            collection.Select(
                document => _entityDocumentBuilder.Build<TDocument>([.. document])
            )
        );

        // update for next interaction
        _from += _request.Limit;

        if (_documents.TryDequeue(out _current)) {
            UpdateTotalCount();
        }
    }

    private static void OnSearchFailure(Error[] errors) {
        throw new InvalidOperationException(errors[0].Message);
    }

    private void UpdateTotalCount() {
        if (_totalCount.HasValue) { return; }

        var count = _index.Count(_request.Query);

        _totalCount = count.Match(
            onSuccess: value => value,
            onFailure: errors => throw new InvalidOperationException(errors[0].Message)
        );
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
