namespace Nameless.Caching {
    public sealed class CacheEntryOptions {
        #region Public Static Read-Only Fields

        public static readonly EvictionCallback EmptyEvictionCallback = (_, __, ___)
            => { };

        #endregion

        #region Public Properties

        public TimeSpan ExpiresIn { get; set; }

        public EvictionCallback? EvictionCallback { get; set; }

        #endregion
    }
}
