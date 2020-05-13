using System;
using System.Collections.Concurrent;

namespace Nameless.Patterns.Singleton {
    public sealed class SingletonFactory : IDisposable {
        #region Private Static Read-Only Fields

        private static readonly ConcurrentDictionary<string, object> Cache = new ConcurrentDictionary<string, object> ();

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Private Constructors

        private SingletonFactory () { }

        #endregion

        #region Destructor

        ~SingletonFactory () {
            Dispose (disposing: false);
        }

        #endregion

        #region Public Static Methods

        public static T Create<T> (params object[] args) where T : class {
            var key = CreateCacheKey (typeof (T), args);
            return (T)Cache.GetOrAdd (key, item => {
                return !args.IsNullOrEmpty ()
                    ? Activator.CreateInstance (typeof (T), args: args)
                    : Activator.CreateInstance (typeof (T));
            });
        }

        #endregion

        #region Private Static Methods

        private static string CreateCacheKey (Type type, object[] args) {
            var hash = 13;
            unchecked {
                foreach (var arg in args) {
                    hash += ((arg != null) ? arg.GetHashCode () : 0) * 7;
                }
            }
            return $"[{hash}]: {type.FullName}";
        }

        #endregion

        #region Private Methods

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (Cache) {
                    foreach (var item in Cache) {
                        if (item.Value is IDisposable disposable) {
                            disposable.Dispose ();
                        }
                    }
                }
            }

            Cache.Clear ();
            _disposed = true;
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