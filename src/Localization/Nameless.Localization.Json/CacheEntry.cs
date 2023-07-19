using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {
    internal class CacheEntry : IDisposable {
        #region Private Fields

        private Translation _translation;
        private IDisposable _changeMonitor;
        private bool _disposed;

        #endregion

        #region Internal Properties

        internal Translation Translation => _translation;

        #endregion

        #region Internal Constructors

        internal CacheEntry(Translation translation, IDisposable changeMonitor) {
            _translation = Prevent.Against.Null(translation, nameof(translation));
            _changeMonitor = Prevent.Against.Null(changeMonitor, nameof(changeMonitor));
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

            _translation = null!;
            _changeMonitor = null!;
            _disposed = true;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
