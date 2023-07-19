using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {

    internal class CacheEntry : IDisposable {

        #region Private Fields

        private Translation? _group;
        private IDisposable? _changeMonitor;
        private bool _disposed;

        #endregion

        #region Internal Properties

        internal Translation Group => _group!;

        #endregion

        #region Internal Constructors

        internal CacheEntry(Translation group, IDisposable changeMonitor) {
            Prevent.Against.Null(group, nameof(group));
            Prevent.Against.Null(changeMonitor, nameof(changeMonitor));

            _group = group;
            _changeMonitor = changeMonitor;
        }

        #endregion

        #region Destructor

        ~CacheEntry() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _changeMonitor?.Dispose();
            }

            _group = default;
            _changeMonitor = default;
            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
