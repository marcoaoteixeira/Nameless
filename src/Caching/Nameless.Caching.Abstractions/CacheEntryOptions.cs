namespace Nameless.Caching {
    /// <summary>
    /// Provides information for cache entries.
    /// </summary>
    public readonly record struct CacheEntryOptions {
        #region Public Properties

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> for expiration.
        /// </summary>
        public TimeSpan ExpiresIn { get; }

        /// <summary>
        /// Gets the callback for eviction action.
        /// </summary>
        public EvictionCallback? EvictionCallback { get; }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="CacheEntryOptions"/>
        /// </summary>
        /// <param name="expiresIn">The <see cref="TimeSpan"/> representing the expiration time.</param>
        /// <param name="evictionCallback">The callback action when eviction occurs.</param>
        public CacheEntryOptions(TimeSpan? expiresIn = null, EvictionCallback? evictionCallback = null) {
            ExpiresIn = expiresIn ?? TimeSpan.Zero;
            EvictionCallback = evictionCallback;
        }

        #endregion
    }
}
