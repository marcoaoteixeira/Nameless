using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Localization.Json.Internals;
using Nameless.Localization.Json.Objects;
using Nameless.Null;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
///     Current implementation of <see cref="IResourceManager" />.
/// </summary>
public sealed class ResourceManager : IResourceManager {
    private readonly ConcurrentDictionary<CacheKey, CacheEntry> _cache = [];
    private readonly IFileProvider _fileProvider;
    private readonly ILogger<ResourceManager> _logger;
    private readonly IOptions<JsonLocalizationOptions> _options;

    /// <summary>
    ///     Initializes a new instance of <see cref="ResourceManager" />.
    /// </summary>
    /// <param name="fileProvider">The file provider.</param>
    /// <param name="options">The localization options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="fileProvider" /> or
    ///     <paramref name="options" /> or
    ///     <paramref name="logger" /> is <see langword="null"/>.
    /// </exception>
    public ResourceManager(IFileProvider fileProvider, IOptions<JsonLocalizationOptions> options, ILogger<ResourceManager> logger) {
        _fileProvider = Guard.Against.Null(fileProvider);
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location" /> or
    ///     <paramref name="culture"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location"/> is empty or white spaces.
    /// </exception>
    public Resource GetResource(string baseName, string location, string culture) {
        Guard.Against.NullOrWhiteSpace(baseName);
        Guard.Against.NullOrWhiteSpace(location);
        Guard.Against.Null(culture);

        _logger.GettingResourceForCulture(culture);

        var cacheKey = CacheKey.Create(baseName, location, culture);
        var entry = _cache.GetOrAdd(cacheKey, CreateCacheEntry);

        return entry.Resource;
    }

    private string GetResourcePathFromCacheKey(CacheKey cacheKey) {
        var extension = !string.IsNullOrWhiteSpace(cacheKey.Culture)
            ? $".{cacheKey.Culture}.json"
            : ".json";

        var parts = cacheKey.Path.Split(Separators.DOT)[..^1]
                            .Prepend(_options.Value.ResourcesFolderName)
                            .Append($"{cacheKey.Location}{extension}")
                            .ToArray();

        return Path.Combine(parts);
    }

    private CacheEntry CreateCacheEntry(CacheKey cacheKey) {
        var path = GetResourcePathFromCacheKey(cacheKey);

        _logger.CreatingCacheEntryForResourceFile(path);

        if (!TryGetResourceFile(path, out var file) ||
            !TryExtractResourceFileContent(file, out var fileContent) ||
            !TryDeserializeFileResourceContent(fileContent, cacheKey.Path, cacheKey.Culture, out var resource)) {
            return CacheEntry.Empty;
        }

        var fileChangeCallback = CreateFileChangeCallback(path, cacheKey);

        return new CacheEntry(resource, fileChangeCallback);
    }

    private bool TryGetResourceFile(string path, [NotNullWhen(true)] out IFileInfo? file) {
        file = _fileProvider.GetFileInfo(path);

        _logger.OnCondition(!file.Exists)
               .ResourceFileNotFound(file.Name);

        return file.Exists;
    }

    private bool TryExtractResourceFileContent(IFileInfo file, [NotNullWhen(true)] out string? content) {
        content = null;

        try {
            using var fileStream = file.CreateReadStream();
            content = fileStream.GetContentAsString();

            if (string.IsNullOrWhiteSpace(content)) {
                _logger.ResourceFileContentIsEmpty(file.Name);

                content = null;

                return false;
            }
        }
        catch (Exception ex) {
            _logger.ErrorReadingResourceFile(file.Name, ex);
        }

        return content is not null;
    }

    private bool TryDeserializeFileResourceContent(string fileContent, string path, string culture,
                                                   [NotNullWhen(true)] out Resource? resource) {
        resource = null;

        try {
            var message = JsonSerializer.Deserialize<Message[]>(fileContent) ?? [];

            resource = new Resource(path, culture, message, true);
        }
        catch (Exception ex) {
            _logger.ResourceDeserializationFailed(path, ex);

            return false;
        }

        return true;
    }

    private IDisposable CreateFileChangeCallback(string path, CacheKey cacheKey) {
        if (!_options.Value.WatchFileForChanges) {
            return NullDisposable.Instance;
        }

        _logger.SettingFileWatcherForResourceFile(path);

        var changeToken = _fileProvider.Watch(path);
        var handler = changeToken.RegisterChangeCallback(RemoveResourceFromCache,
            cacheKey);

        return handler;
    }

    private void RemoveResourceFromCache(object? state) {
        if (state is not CacheKey cacheKey) {
            return;
        }

        if (_cache.TryRemove(cacheKey, out var entry)) {
            entry.FileChangeCallback.Dispose();
        }
    }
}