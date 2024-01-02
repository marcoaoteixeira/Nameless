using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace Nameless.Caching.InMemory {
    public sealed class InMemoryCache : ICache {
        #region Private Read-Only Fields

        private readonly IMemoryCache _memoryCache;

        #endregion

        #region Public Constructors

        public InMemoryCache(IMemoryCache memoryCache) {
            _memoryCache = Guard.Against.Null(memoryCache, nameof(memoryCache));
        }

        #endregion

        #region Private Static Methods

        private static void OnEviction(
            EvictionCallback evictionCallback,
            string key,
            object? value,
            string reason,
            CancellationTokenSource cts
        ) {
            evictionCallback(key, value, reason);
            cts.Dispose();
        }

        private static MemoryCacheEntryOptions CreateMemoryCacheEntryOptions(
            CacheEntryOptions? opts = null
        ) {
            if (opts is null || opts.ExpiresIn == default) {
                return new();
            }

            var cts = new CancellationTokenSource(opts.ExpiresIn);
            var changeToken = new CancellationChangeToken(cts.Token);
            var result = new MemoryCacheEntryOptions();

            result.ExpirationTokens.Add(changeToken);

            result.RegisterPostEvictionCallback((key, value, reason, state)
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

        public Task<bool> SetAsync(
            string key,
            object value,
            CacheEntryOptions? opts = null,
            CancellationToken cancellationToken = default
        ) {
            Guard.Against.NullOrWhiteSpace(key, nameof(key));
            Guard.Against.Null(value, nameof(value));

            cancellationToken.ThrowIfCancellationRequested();

            var result = _memoryCache.Set(
                key,
                value,
                CreateMemoryCacheEntryOptions(opts)
            ) is not null;

            return Task.FromResult(result);
        }

        public Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default
        ) {
            Guard.Against.NullOrWhiteSpace(key, nameof(key));

            cancellationToken.ThrowIfCancellationRequested();

            var value = _memoryCache.Get(key);

            return Task.FromResult(value is T result
                ? result
                : default
            );
        }

        public Task<bool> RemoveAsync(
            string key,
            CancellationToken cancellationToken = default
        ) {
            Guard.Against.NullOrWhiteSpace(key, nameof(key));

            cancellationToken.ThrowIfCancellationRequested();

            _memoryCache.Remove(key);

            return Task.FromResult(true);
        }

        #endregion
    }
}