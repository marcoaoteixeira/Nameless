namespace Nameless.Caching {
    /// <summary>
    /// Extension methods for <see cref="ICacheService"/>
    /// </summary>
    public static class CacheServiceExtension {
        #region Public Static Methods

        /// <summary>
        /// Gets or stores an entity into the underlying cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="self">The current instance of <see cref="ICacheService"/>.</param>
        /// <param name="key">The key to the entity.</param>
        /// <param name="value">The actual entity to be stored.</param>
        /// <param name="opts">The store options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity if found; otherwise <c>default</c>.</returns>
        public static Task<T> GetOrSetAsync<T>(
            this ICacheService self,
            string key,
            T value,
            CacheEntryOptions opts,
            CancellationToken cancellationToken = default)
            => GetOrSetAsync(self, key, _ => value, opts, cancellationToken);

        /// <summary>
        /// Gets or stores an entity into the underlying cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="self">The current instance of <see cref="ICacheService"/>.</param>
        /// <param name="key">The key to the entity.</param>
        /// <param name="valueFactory">The factory responsible for entity creation.</param>
        /// <param name="opts">The store options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity if found; otherwise <c>default</c>.</returns>
        /// <exception cref="InvalidOperationException">
        /// If the <paramref name="valueFactory"/> returns <c>null</c>.
        /// </exception>
        public static async Task<T> GetOrSetAsync<T>(
            this ICacheService self,
            string key,
            Func<string, T> valueFactory,
            CacheEntryOptions opts,
            CancellationToken cancellationToken = default) {
            var current = await self.GetAsync<T>(key, cancellationToken);

            if (current is not null) { return current; }

            current = valueFactory(key)
                   ?? throw new InvalidOperationException("Factory must return non null value.");

            await self.SetAsync(key, current, opts, cancellationToken);

            return current;
        }

        #endregion
    }
}
