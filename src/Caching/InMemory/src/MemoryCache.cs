using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using MS_MemoryCache = System.Runtime.Caching.MemoryCache;

namespace Nameless.Caching.InMemory {
    public sealed class MemoryCache : ICache, IDisposable {
        #region Public Static Read-Only Fields

        /// <summary>
        /// Gets the cache name.
        /// </summary>
        public static readonly string DefaultCacheName = nameof (MemoryCache);

        #endregion

        #region Private Fields

        private MS_MemoryCache _cache;
        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryCache"/>.
        /// </summary>
        public MemoryCache () : this (DefaultCacheName) { }

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryCache"/>.
        /// </summary>
        /// <param name="cacheName">The name of the cache</param>
        public MemoryCache (string cacheName) {
            _cache = new MS_MemoryCache (cacheName);
        }

        #endregion

        #region Destructor

        ~MemoryCache () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_cache != null) {
                    _cache.Dispose ();
                }
            }

            _cache = null;
            _disposed = true;
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (nameof (MemoryCache));
            }
        }

        private void OnChangeTokenCallback (object value) {
            if (value is string key) {
                _cache.Remove (key);
            }
        }

        private void SetEvictionCallback (CacheItemPolicy policy, CacheEntryOptions options) {
            if (options is NotifyCacheEntryOptions opts) {
                policy.RemovedCallback += (args) => opts.EvictionCallback (args.CacheItem.Key, args.CacheItem.Value);
            }
        }

        private void SetChangeTokens (string key, CacheEntryOptions options) {
            if (options is NotifyCacheEntryOptions opts) {
                foreach (var changeToken in opts.ChangeTokens) {
                    changeToken.RegisterChangeCallback (OnChangeTokenCallback, key);
                }
            }
        }

        private void SetTimeExpiration (CacheItemPolicy policy, CacheEntryOptions options) {
            if (options == null) { return; }

            if (options.AbsoluteExpiration.HasValue) {
                policy.AbsoluteExpiration = options.AbsoluteExpiration.Value;
            }

            if (options.SlidingExpiration.HasValue) {
                policy.SlidingExpiration = options.SlidingExpiration.Value;
            }
        }

        #endregion

        #region ICache Members

        public Task<T> GetAsync<T> (string key, CancellationToken token = default) {
            BlockAccessAfterDispose ();

            var value = _cache.Get (key);
            T result = default;
            if (value != null) {
                result = (T) value;
            }
            return Task.FromResult (result);
        }

        public Task RemoveAsync (string key, CancellationToken token = default) {
            BlockAccessAfterDispose ();

            _cache.Remove (key);

            return Task.CompletedTask;
        }

        public Task SetAsync (string key, object value, CacheEntryOptions options = null, CancellationToken token = default) {
            BlockAccessAfterDispose ();

            var policy = new CacheItemPolicy ();

            SetEvictionCallback (policy, options);
            SetChangeTokens (key, options);
            SetTimeExpiration (policy, options);

            _cache.Set (key, value, policy);

            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion
    }
}