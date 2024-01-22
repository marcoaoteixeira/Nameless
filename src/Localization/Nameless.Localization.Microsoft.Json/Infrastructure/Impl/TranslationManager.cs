using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Nameless.Localization.Microsoft.Json.Objects;
using Nameless.Localization.Microsoft.Json.Options;

namespace Nameless.Localization.Microsoft.Json.Infrastructure.Impl {
    public sealed class TranslationManager : ITranslationManager {
        #region Private Records

        private record CacheEntry {
            #region Public Properties

            public string Culture { get; set; } = string.Empty;
            public Translation Translation { get; set; } = Translation.Empty;
            public IDisposable? FileChangeHandler { get; set; }

            #endregion
        }

        #endregion

        #region Private Read-Only Fields

        private readonly ConcurrentDictionary<string, CacheEntry> Cache = [];
        private readonly IFileProvider _fileProvider;
        private readonly LocalizationOptions _options;

        #endregion

        #region Public Constructors

        public TranslationManager(IFileProvider fileProvider)
            : this(fileProvider, LocalizationOptions.Default) { }

        public TranslationManager(IFileProvider fileProvider, LocalizationOptions options) {
            _fileProvider = Guard.Against.Null(fileProvider, nameof(fileProvider));
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Private Methods

        private void FileChangeHandler(object? state) {
            if (state is null) { return; }

            var culture = (string)state;

            if (Cache.TryRemove(culture, out var entry)) {
                entry.FileChangeHandler?.Dispose();
            }
        }

        private bool TranslationFileExists(string culture) {
            var path = GetTranslationFilePath(culture);
            var file = _fileProvider.GetFileInfo(path);

            return file.Exists;
        }

        private string GetTranslationFilePath(string culture)
            => Path.Combine(_options.TranslationFolderName, $"{culture}.json");

        private string GetFileContent(string path) {
            var file = _fileProvider.GetFileInfo(path);
            using var reader = new StreamReader(file.CreateReadStream());
            var content = reader.ReadToEnd();

            return content;
        }

        private IDisposable? CreateFileChangeHandler(string culture, string path) {
            if (!_options.WatchFileForChanges) {
                return null;
            }

            var changeToken = _fileProvider.Watch(path);
            var handler = changeToken
                .RegisterChangeCallback(
                    callback: FileChangeHandler,
                    state: culture
                );

            return handler;
        }

        private CacheEntry CreateCacheEntry(string culture) {
            var path = GetTranslationFilePath(culture);
            var fileContent = GetFileContent(path);
            var fileChangeHandler = CreateFileChangeHandler(culture, path);
            var translation = JsonSerializer.Deserialize<Translation>(fileContent);

            return translation is not null
                ? new() {
                    Culture = culture,
                    Translation = translation,
                    FileChangeHandler = fileChangeHandler
                }
                : throw new InvalidOperationException($"Couldn't deserialize translation file. Culture: {culture}");
        }

        #endregion

        #region ITranslationManager Members

        public Translation GetTranslation(string culture) {
            Guard.Against.NullOrWhiteSpace(culture, nameof(culture));

            if (!TranslationFileExists(culture)) {
                return Translation.Empty;
            }

            var entry = Cache.GetOrAdd(culture, CreateCacheEntry);

            return entry.Translation;
        }

        #endregion
    }
}
