namespace Nameless.Caching {

    public sealed class CacheEntryOptions {

        #region Private Static Read-Only Fields

        private static readonly EvictionCallback EmptyEvictionCallback = (key, value, reason) => { };

        #endregion

        #region Private Fields

        private EvictionCallback? _evictionCallback = null;

        #endregion

        #region Public Properties

        public TimeSpan ExpiresIn { get; set; }
        public EvictionCallback EvictionCallback {
            get { return _evictionCallback ??= EmptyEvictionCallback; }
            set { _evictionCallback = value; }
        }

        #endregion
    }
}
