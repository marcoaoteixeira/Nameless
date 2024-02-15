using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Nameless.Caching.InMemory {
    public sealed class InMemoryCache : ICache, IDisposable {
        #region Private Read-Only Fields

        private readonly MemoryCacheOptions _options;

        #endregion

        #region Private Fields

        private MemoryCache? _memoryCache;
        private bool _disposed;

        #endregion

        #region Public Constructors

        public InMemoryCache()
            : this(new()) { }

        public InMemoryCache(MemoryCacheOptions options) {
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Static Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(InMemoryCache));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) {
                return;
            }

            if (disposing) {
                _memoryCache?.Dispose();
            }

            _memoryCache = null;
            _disposed = true;
        }

        private IMemoryCache GetMemoryCache()
            => _memoryCache ??= new MemoryCache(_options);

        private static void OnEviction(EvictionCallback evictionCallback, string key, object? value, string reason, CancellationTokenSource cts) {
            evictionCallback(key, value, reason);
            cts.Dispose();
        }

        private static MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(CacheEntryOptions? opts) {
            if (opts is null || opts.ExpiresIn == default) {
                return new();
            }

            var cts = new CancellationTokenSource(opts.ExpiresIn);
            var changeToken = new CancellationChangeToken(cts.Token);
            var result = new MemoryCacheEntryOptions();

            result.ExpirationTokens.Add(changeToken);

            result.RegisterPostEvictionCallback((key, value, reason, _)
                => OnEviction(
                    opts.EvictionCallback,
                    (string)key,
                    value,
                    reason.ToString(),
                    cts
                ));

            return result;
        }

        #endregion

        #region ICache Members

        public Task<bool> SetAsync(string key, object value, CacheEntryOptions? opts, CancellationToken cancellationToken) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Guard.Against.Null(value, nameof(value));

            cancellationToken.ThrowIfCancellationRequested();

            var entry = GetMemoryCache().Set(
                key,
                value,
                CreateMemoryCacheEntryOptions(opts)
            );

            return Task.FromResult(entry is not null);
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(key, nameof(key));

            cancellationToken.ThrowIfCancellationRequested();

            var value = GetMemoryCache().Get(key);

            return Task.FromResult(value is T result
                ? result
                : default
            );
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            Guard.Against.NullOrWhiteSpace(key, nameof(key));

            cancellationToken.ThrowIfCancellationRequested();

            GetMemoryCache().Remove(key);

            return Task.FromResult(true);
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}