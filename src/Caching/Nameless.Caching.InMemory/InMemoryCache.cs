using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Nameless.Caching.InMemory {

    public sealed class InMemoryCache : ICache, IDisposable {

        #region Private Fields

        private IMemoryCache _cache;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public InMemoryCache(MemoryCacheOptions options) {
            _cache = new MemoryCache(options);
        }

        #endregion

        #region Destructor

        ~InMemoryCache() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static void OnEviction(EvictionCallback evictionCallback, string key, object? value, string reason, CancellationTokenSource cts) {
            evictionCallback(key, value, reason);
            cts.Dispose();
        }

        private static MemoryCacheEntryOptions GetOptions(CacheEntryOptions? opts = default) {
            var result = new MemoryCacheEntryOptions();

            if (opts == null) { return result; }

            if (opts.ExpiresIn != default) {
                var cts = new CancellationTokenSource(opts.ExpiresIn);
                var changeToken = new CancellationChangeToken(cts.Token);
                result.ExpirationTokens.Add(changeToken);

                result.RegisterPostEvictionCallback((key, value, reason, state) => {
                    OnEviction(opts.EvictionCallback, (string)key, value, reason.ToString(), cts);
                });
            }

            return result;
        }

        #endregion

        #region Private Methods

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _cache.Dispose();
            }

            _cache = default!;
            _disposed = true;
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        #endregion

        #region ICache Members

        public Task<object> SetAsync(string key, object value, CacheEntryOptions? opts = default, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var result = _cache.Set(key, value, GetOptions(opts));

            return Task.FromResult(result);
        }

        public Task<object?> GetAsync(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            var value = _cache.Get(key);

            return Task.FromResult(value);
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            cancellationToken.ThrowIfCancellationRequested();

            _cache.Remove(key);

            return Task.FromResult(true);
        }

        public Task<bool> RefreshAsync(string key, CancellationToken cancellationToken = default) => Task.FromResult(false);

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}