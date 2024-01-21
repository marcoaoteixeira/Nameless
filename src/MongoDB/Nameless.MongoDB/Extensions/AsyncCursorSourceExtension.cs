using MongoDB.Driver;

namespace Nameless.MongoDB {
    public static class AsyncCursorSourceExtension {
        #region Private Inner Classes

        private class AsyncEnumerableAdapter<T> : IAsyncEnumerable<T> {
            #region Private Read-Only Fields

            private readonly IAsyncCursorSource<T> _source;

            #endregion

            #region Internal Constructors

            internal AsyncEnumerableAdapter(IAsyncCursorSource<T> source) {
                _source = Guard.Against.Null(source, nameof(source));
            }

            #endregion

            #region IAsyncEnumerable<T> Members

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
                => new AsyncEnumeratorAdapter<T>(_source, cancellationToken);

            #endregion
        }

        private class AsyncEnumeratorAdapter<T> : IAsyncEnumerator<T> {
            #region Private Read-Only Fields

            private readonly IAsyncCursorSource<T> _source;
            private readonly CancellationToken _cancellationToken;

            #endregion

            #region Private Fields

            private IAsyncCursor<T>? _asyncCursor;
            private IEnumerator<T>? _batchEnumerator;

            #endregion

            #region Internal Constructors

            internal AsyncEnumeratorAdapter(IAsyncCursorSource<T> source, CancellationToken cancellationToken) {
                _source = source;
                _cancellationToken = cancellationToken;
            }

            #endregion

            #region IAsyncEnumerator<T> Members

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

            #endregion
        }

        #endregion
    }
}
