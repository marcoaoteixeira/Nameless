﻿namespace Nameless.Caching {

    public sealed class CacheEntryOptions {

        #region Private Static Read-Only Fields

        private static readonly EvictionCallback DefaultEvictionCallback = (key, value, reason) => { };

        #endregion

        #region Private Fields

        private EvictionCallback? _evictionCallback = default;

        #endregion

        #region Public Properties

        public TimeSpan ExpiresIn { get; set; } = TimeSpan.Zero;
        public EvictionCallback EvictionCallback {
            get { return _evictionCallback ??= DefaultEvictionCallback; }
            set { _evictionCallback = value; }
        }

        #endregion
    }
}