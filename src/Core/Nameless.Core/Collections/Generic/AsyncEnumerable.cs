﻿namespace Nameless.Collections.Generic {
    public sealed class AsyncEnumerable<T> : IAsyncEnumerable<T> {
        #region Private Read-Only Fields

        private readonly IEnumerable<T> _enumerable;

        #endregion

        #region Public Constructors

        public AsyncEnumerable(IEnumerable<T> enumerable) {
            Guard.Against.Null(enumerable, nameof(enumerable));

            _enumerable = enumerable;
        }

        #endregion

        #region IAsyncEnumerable<T> Members

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
            => new AsyncEnumerator<T>(_enumerable, cancellationToken);

        #endregion
    }

    public sealed class AsyncEnumerator<T> : IAsyncEnumerator<T> {
        #region Private Read-Only Fields

        private readonly CancellationToken _cancellationToken;

        #endregion

        #region Private Fields

        private IEnumerator<T>? _enumerator;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public AsyncEnumerator(IEnumerable<T> enumerable, CancellationToken cancellationToken = default) {
            Guard.Against.Null(enumerable, nameof(enumerable));

            _enumerator = enumerable.GetEnumerator();
            _cancellationToken = cancellationToken;
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(typeof(AsyncEnumerable<>).Name);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _enumerator?.Dispose();
            }

            _enumerator = null;
            _disposed = true;
        }

        #endregion

        #region IAsyncEnumerator<T> Members

        public T Current {
            get {
                BlockAccessAfterDispose();

                _cancellationToken.ThrowIfCancellationRequested();
                return _enumerator!.Current;
            }
        }

        public ValueTask DisposeAsync() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);

            return default;
        }

        public ValueTask<bool> MoveNextAsync() {
            BlockAccessAfterDispose();

            _cancellationToken.ThrowIfCancellationRequested();

            var result = _enumerator!.MoveNext();

            return new ValueTask<bool>(result);
        }

        #endregion
    }
}
