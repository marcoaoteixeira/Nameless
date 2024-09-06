using MongoDB.Driver;

namespace Nameless.MongoDB.Internals;

internal class AsyncEnumeratorAdapter<T> : IAsyncEnumerator<T> {
    private readonly IAsyncCursorSource<T> _source;
    private readonly CancellationToken _cancellationToken;

    private IAsyncCursor<T>? _asyncCursor;
    private IEnumerator<T>? _batchEnumerator;

    internal AsyncEnumeratorAdapter(IAsyncCursorSource<T> source, CancellationToken cancellationToken) {
        _source = source;
        _cancellationToken = cancellationToken;
    }

    public T Current {
        get {
            // TODO: Check if this code really run ok
            if (_batchEnumerator is null || _batchEnumerator.Current is null) {
                throw new NullReferenceException();
            }
            return _batchEnumerator.Current;
        }
    }

    public async ValueTask<bool> MoveNextAsync() {
        _asyncCursor ??= await _source.ToCursorAsync(_cancellationToken);

        if (_batchEnumerator is not null && _batchEnumerator.MoveNext()) {
            return true;
        }

        if (_asyncCursor is not null && await _asyncCursor.MoveNextAsync(_cancellationToken)) {
            _batchEnumerator?.Dispose();
            _batchEnumerator = _asyncCursor.Current.GetEnumerator();

            return _batchEnumerator.MoveNext();
        }

        return false;
    }

    public ValueTask DisposeAsync() {
        _asyncCursor?.Dispose();
        _asyncCursor = null;

        return default;
    }
}
