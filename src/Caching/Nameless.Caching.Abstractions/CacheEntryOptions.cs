namespace Nameless.Caching {
    public sealed class CacheEntryOptions {
        #region Private Static Read-Only Fields

        private static readonly EvictionCallback EmptyEvictionCallback = (key, value, reason) => { };

        #endregion

        #region Public Properties

        public TimeSpan ExpiresIn { get; set; }

        private EvictionCallback? _evictionCallback = null;
        public EvictionCallback EvictionCallback {
            get => _evictionCallback ??= EmptyEvictionCallback;
            set => _evictionCallback = value;
        }

        #endregion
    }
}
