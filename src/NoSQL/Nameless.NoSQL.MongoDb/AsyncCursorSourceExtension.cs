﻿using MongoDB.Driver;

namespace Nameless.NoSQL.MongoDb {

    public static class AsyncCursorSourceExtension {

        #region Private Inner Classes

        private class AsyncEnumerableAdapter<T> : IAsyncEnumerable<T> {

            #region Private Read-Only Fields

            private readonly IAsyncCursorSource<T> _source;

            #endregion

            #region Internal Constructors

            internal AsyncEnumerableAdapter(IAsyncCursorSource<T> source) {
                Prevent.Null(source, nameof(source));

                _source = source;
            }

            #endregion

            #region IAsyncEnumerable<T> Members

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken) {
                return new AsyncEnumeratorAdapter<T>(_source, cancellationToken);
            }

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
                    if (_batchEnumerator == default || _batchEnumerator.Current == null) {
                        throw new NullReferenceException();
                    }
                    return _batchEnumerator.Current;
                }
            }

            public async ValueTask<bool> MoveNextAsync() {
                if (_asyncCursor == default) {
                    _asyncCursor = await _source.ToCursorAsync(_cancellationToken);
                }

                if (_batchEnumerator != default && _batchEnumerator.MoveNext()) {
                    return true;
                }

                if (_asyncCursor != default && await _asyncCursor.MoveNextAsync(_cancellationToken)) {
                    _batchEnumerator?.Dispose();
                    _batchEnumerator = _asyncCursor.Current.GetEnumerator();

                    return _batchEnumerator.MoveNext();
                }

                return false;
            }

            public ValueTask DisposeAsync() {
                _asyncCursor?.Dispose();
                _asyncCursor = default;

                return ValueTask.CompletedTask;
            }

            #endregion
        }

        #endregion
    }
}