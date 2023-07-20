using System.Globalization;
using System.Text.Json;
using Nameless.FileStorage;
using Nameless.Localization.Json.Objects;
using Nameless.Localization.Json.Objects.Translation;

namespace Nameless.Localization.Json.Services.Impl
{
    public sealed class FileTranslationProvider : ITranslationProvider, IDisposable {
        #region Private Static Read-Only Fields

        private static readonly SemaphoreSlim Semaphore = new(1, 1);
        private static readonly Dictionary<string, CacheEntry> Cache = new();

        #endregion

        #region Private Read-Only Fields

        private readonly IFileStorage _fileStorage;
        private readonly LocalizationOptions _options;

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Constructors

        public FileTranslationProvider(IFileStorage fileStorage, LocalizationOptions? options = null) {
            _fileStorage = Prevent.Against.Null(fileStorage, nameof(fileStorage));

            _options = options ?? LocalizationOptions.Default;
        }

        #endregion

        #region Destructor

        ~FileTranslationProvider() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static Trunk DeserializeTranslationFile(string json) {
            var options = new JsonSerializerOptions {
                Converters = { TrunkJsonConverter.Default }
            };

            var result = JsonSerializer.Deserialize<Trunk>(json, options);

            return result!;
        }

        private static async Task<string> GetTranslationFileContentAsync(IFile file, CancellationToken cancellationToken) {
            // If the file exists, reads it.
            string result;
            using (var stream = await file.OpenAsync(cancellationToken)) {
                result = stream.ToText();
            }
            return result;
        }

        #endregion

        #region Private Methods

        private IDisposable CreateChangeMonitor(IFile file) {
            // Keep an eye in the file changed event, if needed.
            return _options.ReloadOnChange ? file.Watch(ChangeMonitorCallback) : NullDisposable.Instance;
        }

        private async Task<IFile> GetTranslationFileAsync(string key, CancellationToken cancellationToken) {
            // Gets the translation file by key => culture name.
            var relativeFilePath = Path.Combine(_options.ResourceFolderName, $"{key}.json"); // e.g: pt-br.json
            var file = await _fileStorage.GetFileAsync(relativeFilePath, cancellationToken);
            return file;
        }

        private void ChangeMonitorCallback(ChangeEventArgs args) {
            var culture = Path.GetFileNameWithoutExtension(args.OriginalPath).ToLowerInvariant(); // normalize
            lock (Cache) {
                if (Cache.Remove(culture, out var entry)) {
                    ((IDisposable)entry).Dispose();
                }
            }
        }

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(FileTranslationProvider));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                lock (Cache) {
                    foreach (var kvp in Cache) {
                        ((IDisposable)kvp.Value).Dispose();
                    }
                }
            }

            Cache.Clear();

            _disposed = true;
        }

        #endregion

        #region ITranslationProvider Members

        public async Task<Trunk> GetAsync(CultureInfo culture, CancellationToken cancellationToken = default) {
            BlockAccessAfterDispose();

            Prevent.Against.Null(culture, nameof(culture));

            try {
                // Sync
                await Semaphore.WaitAsync(cancellationToken);

                var key = culture.Name;

                // If the cache already holds a reference to the culture,
                // just returns it.
                if (Cache.TryGetValue(key, out var entry)) {
                    return entry.Trunk;
                }

                var file = await GetTranslationFileAsync(key, cancellationToken);

                // If file exists, process it and create the cache entry
                if (file.Exists) {
                    var json = await GetTranslationFileContentAsync(file, cancellationToken);
                    var translation = DeserializeTranslationFile(json);
                    var changeMonitor = CreateChangeMonitor(file);

                    entry = new CacheEntry(translation, changeMonitor);
                } else {
                    // If file doesn't exists, create a dummy cache entry
                    entry = new CacheEntry(new Trunk(culture), NullDisposable.Instance);
                }

                // Assert entry inside cache
                Cache.AddOrChange(key, entry);

                return entry.Trunk;
            } finally { Semaphore.Release(); }
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
