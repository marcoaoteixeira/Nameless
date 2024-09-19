using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Localization.Json.Internals;
using Nameless.Localization.Json.Objects;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json.Infrastructure.Impl;

public sealed class TranslationManager : ITranslationManager {
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = [];
    private readonly IFileProvider _fileProvider;
    private readonly LocalizationOptions _options;
    private readonly ILogger<TranslationManager> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="TranslationManager"/>.
    /// </summary>
    /// <param name="fileProvider">The file provider.</param>
    /// <param name="options">The localization options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="fileProvider"/> or
    /// <paramref name="options"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public TranslationManager(IFileProvider fileProvider, IOptions<LocalizationOptions> options, ILogger<TranslationManager> logger) {
        _fileProvider = Prevent.Argument.Null(fileProvider);
        _options = Prevent.Argument.Null(options).Value;
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    public Translation GetTranslation(string culture) {
        Prevent.Argument.NullOrWhiteSpace(culture);

        var entry = _cache.GetOrAdd(culture, CreateCacheEntry);

        return entry.Translation;
    }

    private CacheEntry CreateCacheEntry(string culture) {
        var path = Path.Combine(_options.TranslationFolderName, $"{culture}.json");

        if (!TryGetTranslationFile(path, out var file)) {
            return CacheEntry.Empty;
        }

        if (!TryGetTranslationFileContent(file, out var fileContent)) {
            return CacheEntry.Empty;
        }
        
        if (!TryDeserializeTranslationObject(fileContent, file.Name, out var translation)) {
            return CacheEntry.Empty;
        }

        var fileChangeCallback = CreateFileChangeCallback(path, culture);

        return new CacheEntry(translation, fileChangeCallback);
    }

    private bool TryGetTranslationFile(string path, [NotNullWhen(returnValue: true)] out IFileInfo? file) {
        file = _fileProvider.GetFileInfo(path);

        if (!file.Exists) {
            LoggerHandlers.TranslationFileNotFound(_logger,
                                                 file.Name,
                                                 null /* exception */);
            file = null;
        }

        return file is not null;
    }

    private bool TryGetTranslationFileContent(IFileInfo file, [NotNullWhen(returnValue: true)] out string? content) {
        content = null;

        try {
            using var fileStream = file.CreateReadStream();
            content = fileStream.ToText();

            if (string.IsNullOrWhiteSpace(content)) {
                LoggerHandlers.TranslationFileEmpty(_logger,
                                                  file.Name,
                                                  null /* exception */);
                content = null;
                return false;
            }
        } catch (Exception ex) {
            LoggerHandlers.ReadTranslationFileContentFailure(_logger,
                                                           file.Name,
                                                           ex);
        }

        return content is not null;
    }

    private bool TryDeserializeTranslationObject(string fileContent, string fileName, [NotNullWhen(returnValue: true)]out Translation? translation) {
        translation = null;
        
        try { translation = JsonSerializer.Deserialize<Translation>(fileContent); }
        catch (Exception ex) {
            LoggerHandlers.TranslationObjectDeserializationFailure(_logger,
                                                                 fileName,
                                                                 ex);
            return false;
        }

        if (translation is null) {
            LoggerHandlers.JsonSerializerReturnNullTranslationObject(_logger,
                                                                   fileName,
                                                                   null /* exception */);
        }

        return translation is not null;
    }

    private IDisposable CreateFileChangeCallback(string path, string culture) {
        if (!_options.WatchFileForChanges) {
            return NullDisposable.Instance;
        }

        var changeToken = _fileProvider.Watch(path);
        var handler = changeToken
            .RegisterChangeCallback(callback: HandleFileChange,
                                    state: culture);

        return handler;
    }

    private void HandleFileChange(object? state) {
        if (state is null) { return; }

        var culture = (string)state;

        if (_cache.TryRemove(culture, out var entry)) {
            entry.FileChangeCallback.Dispose();
        }
    }
}