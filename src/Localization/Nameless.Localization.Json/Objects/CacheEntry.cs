using Nameless.Localization.Json.Objects.Translation;

namespace Nameless.Localization.Json.Objects {
    internal class CacheEntry : IDisposable {
        #region Private Fields

        private Trunk _trunk;
        private IDisposable _changeMonitor;
        private bool _disposed;

        #endregion

        #region Internal Properties

        internal Trunk Trunk => _trunk;

        #endregion

        #region Internal Constructors

        internal CacheEntry(Trunk trunk, IDisposable changeMonitor) {
            _trunk = Prevent.Against.Null(trunk, nameof(trunk));
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
                _changeMonitor.Dispose();
            }

            _trunk = null!;
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
