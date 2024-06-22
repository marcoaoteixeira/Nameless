namespace Nameless.Caching {
    public readonly record struct CacheEntryOptions {
        #region Public Properties

        public TimeSpan ExpiresIn { get; }
        public EvictionCallback? EvictionCallback { get; }

        #endregion

        #region Public Constructors

        public CacheEntryOptions(TimeSpan? expiresIn = null, EvictionCallback? evictionCallback = null) {
            ExpiresIn = expiresIn ?? TimeSpan.Zero;
            EvictionCallback = evictionCallback;
        }

        #endregion
    }
}
