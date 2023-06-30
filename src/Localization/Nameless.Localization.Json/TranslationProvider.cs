﻿using System.Collections.Concurrent;
using System.Globalization;
using Nameless.FileStorage;
using Nameless.Localization.Json.Schema;
using Newtonsoft.Json;

namespace Nameless.Localization.Json {

    public sealed class TranslationProvider : ITranslationProvider, IDisposable {

        #region Private Read-Only Fields

        private readonly IFileStorage _fileStorage;
        private readonly LocalizationOptions _options;

        #endregion

        #region Private Fields

        private ConcurrentDictionary<string, CacheEntry> _cache = new();
        private bool _disposed;

        #endregion

        #region Public Constructors

        public TranslationProvider(IFileStorage fileStorage, LocalizationOptions options) {
            Garda.Prevent.Null(fileStorage, nameof(fileStorage));

            _fileStorage = fileStorage;
            _options = options ?? LocalizationOptions.Default;
        }

        #endregion

        #region Destructor

        ~TranslationProvider() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void ChangeMonitorCallback(ChangeEventArgs args) {
            var culture = Path.GetFileNameWithoutExtension(args.OriginalPath).ToLower(); // normalize
            if (_cache.TryRemove(culture, out var entry)) {
                entry.Dispose();
            }
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (_cache) {
                    foreach (var entry in _cache.Values) {
                        entry.Dispose();
                    }
                }
            }

            _cache.Clear();
            _cache = default!;
            _disposed = true;
        }

        #endregion

        #region ITranslationStorage Members

        public async Task<Translation> GetAsync(CultureInfo culture, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            Garda.Prevent.Null(culture, nameof(culture));

            // Retrieves the associated file
            var relativeFilePath = Path.Combine(_options.ResourceFolderName, $"{culture.Name}.json");
            var file = await _fileStorage.GetFileAsync(relativeFilePath, cancellationToken);

            // If file doesn't exists, return.
            if (!file.Exists) { return new Translation(culture); }

            var entry = _cache.GetOrAdd(culture.Name, key => {
                using var fileStream = file.Open();
                var json = fileStream.ToText();
                var translationCollections = JsonConvert.DeserializeObject<TranslationCollection[]>(json, TranslationCollectionJsonConverter.Default);
                var translation = new Translation(culture, translationCollections);

                // Keep an eye in the file changed event, if needed.
                var changeMonitor = _options.ReloadOnChange ? file.Watch(ChangeMonitorCallback) : NullDisposable.Instance;

                return new CacheEntry(translation, changeMonitor);
            });

            return entry.Group;
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
