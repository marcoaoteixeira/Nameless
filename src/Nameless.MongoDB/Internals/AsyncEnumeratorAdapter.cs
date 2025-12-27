using MongoDB.Driver;

namespace Nameless.MongoDB.Internals;

/// <summary>
/// An adapter for <see cref="IAsyncCursorSource{T}"/> to <see cref="IAsyncEnumerator{T}"/>.
/// </summary>
/// <typeparam name="TValue">Type of the value.</typeparam>
internal class AsyncEnumeratorAdapter<TValue> : IAsyncEnumerator<TValue> {
    private readonly CancellationToken _cancellationToken;
    private readonly IAsyncCursorSource<TValue> _source;

    private IAsyncCursor<TValue>? _asyncCursor;
    private IEnumerator<TValue>? _batchEnumerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncEnumeratorAdapter{TValue}"/> class.
    /// </summary>
    /// <param name="source">The source of the async cursor.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal AsyncEnumeratorAdapter(IAsyncCursorSource<TValue> source, CancellationToken cancellationToken) {
        _source = source;
        _cancellationToken = cancellationToken;
    }

    /// <inheritdoc />
    public TValue Current {
        get {
            // TODO: Check if this code really run ok
            if (_batchEnumerator is null || _batchEnumerator.Current is null) {
                throw new NullReferenceException();
            }

            return _batchEnumerator.Current;
        }
    }

    /// <inheritdoc />
    public async ValueTask<bool> MoveNextAsync() {
        _asyncCursor ??= await _source.ToCursorAsync(_cancellationToken)
                                      .ConfigureAwait(continueOnCapturedContext: false);

        if (_batchEnumerator is not null && _batchEnumerator.MoveNext()) {
            return true;
        }

        var canMoveNext = await _asyncCursor.MoveNextAsync(_cancellationToken)
                                            .ConfigureAwait(continueOnCapturedContext: false);

        if (_asyncCursor is not null && canMoveNext) {
            _batchEnumerator?.Dispose();
            _batchEnumerator = _asyncCursor.Current.GetEnumerator();

            return _batchEnumerator.MoveNext();
        }

        return false;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() {
        _asyncCursor?.Dispose();
        _asyncCursor = null;

        return default;
    }
}