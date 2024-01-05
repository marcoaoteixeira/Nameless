namespace Nameless.Caching {
    public static class CacheExtension {
        #region Public Static Methods

        public static Task<T> GetOrAddAsync<T>(this ICache self, string key, T value, CacheEntryOptions? opts = null, CancellationToken cancellationToken = default)
            => GetOrAddAsync(self, key, (key) => value, opts, cancellationToken);

        public static async Task<T> GetOrAddAsync<T>(this ICache self, string key, Func<string, T> valueFactory, CacheEntryOptions? opts = null, CancellationToken cancellationToken = default) {
            var current = await self.GetAsync<T>(key, cancellationToken);

            if (current is null) {
                current = valueFactory(key) ?? throw new InvalidOperationException("Factory must return non null value.");

                await self.SetAsync(key, current, opts, cancellationToken);
            }

            return current;
        }

        #endregion
    }
}
