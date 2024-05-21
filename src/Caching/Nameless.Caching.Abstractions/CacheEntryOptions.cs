namespace Nameless.Caching {
    public sealed class CacheEntryOptions {
        #region Public Static Read-Only Properties

        public static CacheEntryOptions Empty => new();
        public static EvictionCallback EmptyEvictionCallback { get; } = (_, _, _) => { };

        #endregion

        #region Public Properties

        public TimeSpan ExpiresIn { get; set; }

        public EvictionCallback? EvictionCallback { get; set; }

        #endregion
    }
}
