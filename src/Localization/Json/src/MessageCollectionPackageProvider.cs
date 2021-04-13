using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nameless.FileStorage;
using Nameless.Helpers;
using Nameless.Localization.Json.Schemas;
using Nameless.Logging;
using Newtonsoft.Json;

namespace Nameless.Localization.Json {
    /// <summary>
    /// Holds all mechanics to read the localization files.
    /// For performance sake, use only one instance of this class
    /// through the application life time.
    /// </summary>
    public sealed class MessageCollectionPackageProvider : IMessageCollectionPackageProvider, IDisposable {
        #region Private Read-Only Fields

        private readonly IFileStorage _fileStorage;
        private readonly LocalizationSettings _settings;
        #endregion

        #region Private Fields

        private ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry> ();
        private bool _disposed;

        #endregion

        #region Public Properties

#pragma warning disable IDE0074
        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }
#pragma warning restore IDE0074

        #endregion

        #region Public Constructors

        public MessageCollectionPackageProvider (IFileStorage fileStorage, LocalizationSettings settings = null) {
            Prevent.ParameterNull (fileStorage, nameof (fileStorage));

            _fileStorage = fileStorage;
            _settings = settings ?? new LocalizationSettings ();
        }

        #endregion

        #region Destructor

        ~MessageCollectionPackageProvider () {
            Dispose (disposing: false);
        }

        #endregion

        #region Private Methods

        private void ChangeMonitorCallback (ChangeEventArgs args) {
            var cultureName = Path.GetFileNameWithoutExtension (args.OriginalPath).ToLower (); // normalize

            if (!string.IsNullOrWhiteSpace (cultureName)) {
                if (_cache.TryRemove (cultureName, out CacheEntry entry)) {
                    entry.Dispose ();
                }
            }
        }

        private void BlockAccessAfterDispose () {
            if (_disposed) {
                throw new ObjectDisposedException (GetType ().FullName);
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_cache) {
                    foreach (var entry in _cache.Values) {
                        entry.Dispose ();
                    }
                }
            }

            _cache.Clear ();
            _cache = null;
            _disposed = true;
        }

        #endregion

        #region IMessageCollectionAggregationProvider Members

        public async Task<MessageCollectionPackage> CreateAsync (string cultureName, CancellationToken token = default) {
            BlockAccessAfterDispose ();

            Prevent.ParameterNullOrWhiteSpace (cultureName, nameof (cultureName));

            cultureName = cultureName.ToLower (); // normalize

            // Checks if is a valid culture name.
            if (!CultureInfoHelper.TryGetCultureInfo (cultureName, out _)) {
                Logger.Debug ($"Couldn't find culture {cultureName}");
                return null;
            }

            // Retrieves the associated file
            var filePath = Path.Combine (_settings.ResourceFolderPath, $"{cultureName}.json");
            var file = await _fileStorage.GetFileAsync (filePath);

            token.ThrowIfCancellationRequested ();

            // If file not exists, return.
            if (file == null || !file.Exists) { return null; }

            var entry = _cache.GetOrAdd (cultureName, key => {
                var json = file.GetText (Encoding.UTF8); /* always read files as UTF-8 */
                var messageCollections = JsonConvert.DeserializeObject<MessageCollection[]> (json, new MessageCollectionJsonConverter ());
                var messageCollectionPackage = new MessageCollectionPackage (cultureName, messageCollections);

                // Keep an eye in the file changed event, if needed.
                var changeMonitor = _settings.ReloadOnChange ? file.Watch (ChangeMonitorCallback) : null;

                return CacheEntry.Create (messageCollectionPackage, changeMonitor);
            });

            return entry.GetPackage ();
        }

        #endregion

        #region IDisposable Members

        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion

        #region Private Classes

        private class CacheEntry : IDisposable {
            #region Private Fields

            private MessageCollectionPackage _package;
            private IDisposable _changeMonitor;

            #endregion

            #region Private Constructors

            private CacheEntry (MessageCollectionPackage package, IDisposable changeMonitor) {
                _package = package;
                _changeMonitor = changeMonitor;
            }

            #endregion

            #region Internal Static Methods

            internal static CacheEntry Create (MessageCollectionPackage package, IDisposable changeMonitor = null) {
                return new CacheEntry (package, changeMonitor ?? NullDisposable.Instance);
            }

            #endregion

            #region Internal Methods

            internal MessageCollectionPackage GetPackage () {
                return _package;
            }

            #endregion

            #region IDisposable Members

            public void Dispose () {
                _package = null;
                _changeMonitor.Dispose ();
                _changeMonitor = null;
            }

            #endregion
        }

        #endregion
    }
}