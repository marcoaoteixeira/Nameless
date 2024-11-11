using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Localization.Json.Objects;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json.Infrastructure;

/// <summary>
/// Current implementation of <see cref="IResourceManager"/>.
/// </summary>
public sealed class ResourceManager : IResourceManager {
    private readonly IFileProvider _fileProvider;
    private readonly IOptions<LocalizationOptions> _options;
    private readonly ILogger<ResourceManager> _logger;
    private readonly ConcurrentDictionary<CacheKey, CacheEntry> _cache = [];

    /// <summary>
    /// Initializes a new instance of <see cref="ResourceManager"/>.
    /// </summary>
    /// <param name="fileProvider">The file provider.</param>
    /// <param name="options">The localization options.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="fileProvider"/> or
    /// <paramref name="options"/> or
    /// <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public ResourceManager(IFileProvider fileProvider, IOptions<LocalizationOptions> options, ILogger<ResourceManager> logger) {
        _fileProvider = Prevent.Argument.Null(fileProvider);
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    public Resource GetResource(string baseName, string location, string culture) {
        Prevent.Argument.NullOrWhiteSpace(baseName);
        Prevent.Argument.NullOrWhiteSpace(location);
        Prevent.Argument.Null(culture);

        _logger.GettingResourceForCulture(culture);

        var cacheKey = CacheKey.Create(baseName, location, culture);
        var entry = _cache.GetOrAdd(cacheKey, CreateCacheEntry);

        return entry.Resource;
    }

    private string GetResourcePathFromCacheKey(CacheKey cacheKey) {
        var extension = !string.IsNullOrWhiteSpace(cacheKey.Culture)
            ? $".{cacheKey.Culture}.json"
            : ".json";

        var parts = cacheKey.Path.Split(Constants.Separators.DOT)[..^1]
                                 .Prepend(_options.Value.ResourcesFolderName)
                                 .Append($"{cacheKey.Location}{extension}")
                                 .ToArray();

        return Path.Combine(parts);
    }

    private CacheEntry CreateCacheEntry(CacheKey cacheKey) {
        var path = GetResourcePathFromCacheKey(cacheKey);

        _logger.CreatingCacheEntryForResourceFile(path);

        if (!TryGetResourceFile(path, out var file)) {
            return CacheEntry.Empty;
        }

        if (!TryGetResourceFileContent(file, out var fileContent)) {
            return CacheEntry.Empty;
        }
        
        if (!TryDeserializeResource(fileContent, cacheKey.Path, cacheKey.Culture, out var resource)) {
            return CacheEntry.Empty;
        }

        var fileChangeCallback = CreateFileChangeCallback(path, cacheKey);

        return new CacheEntry(resource, fileChangeCallback);
    }

    private bool TryGetResourceFile(string path, [NotNullWhen(returnValue: true)] out IFileInfo? file) {
        file = _fileProvider.GetFileInfo(path);

        if (!file.Exists) {
            _logger.ResourceFileNotFound(file.Name);
            
            file = null;
        }

        return file is not null;
    }

    private bool TryGetResourceFileContent(IFileInfo file, [NotNullWhen(returnValue: true)] out string? content) {
        content = null;

        try {
            using var fileStream = file.CreateReadStream();
            content = fileStream.ToText();

            if (string.IsNullOrWhiteSpace(content)) {
                _logger.ResourceFileContentIsEmpty(file.Name);

                content = null;

                return false;
            }
        } catch (Exception ex) {
            _logger.ErrorReadingResourceFile(file.Name, ex);
        }

        return content is not null;
    }

    private bool TryDeserializeResource(string fileContent, string path, string culture, [NotNullWhen(returnValue: true)]out Resource? resource) {
        resource = null;

        try {
            resource = new Resource(path,
                                    culture,
                                    JsonSerializer.Deserialize<Message[]>(fileContent) ?? [],
                                    isAvailable: true);
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
        var handler = changeToken.RegisterChangeCallback(callback: RemoveResourceFromCache,
                                                         state: cacheKey);

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