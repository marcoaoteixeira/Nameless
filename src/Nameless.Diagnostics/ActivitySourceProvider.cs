using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Nameless.Diagnostics;

/// <summary>
///     The default implementation of <see cref="IActivitySourceProvider"/>.
/// </summary>
public class ActivitySourceProvider : IActivitySourceProvider {
    private readonly IOptions<ActivitySourceOptions> _options;
    private readonly ConcurrentDictionary<CacheKey, ActivitySourceWrapper> _cache = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ActivitySourceProvider"/> class.
    /// </summary>
    /// <param name="options">
    ///     The options.
    /// </param>
    public ActivitySourceProvider(IOptions<ActivitySourceOptions> options) {
        _options = options;
    }

    /// <inheritdoc />
    public IActivitySource Create(string name, string? version = null) {
        var key = _options.Value.UseUniqueActivitySource
            ? new CacheKey(_options.Value.UniqueActivitySourceName, _options.Value.UniqueActivitySourceVersion)
            : new CacheKey(name, version);

        return _cache.GetOrAdd(key, CreateActivitySource);
    }

    private static ActivitySourceWrapper CreateActivitySource(CacheKey key) {
        return new ActivitySourceWrapper(
            new ActivitySource(key.Name, key.Version ?? string.Empty)
        );
    }

    private readonly record struct CacheKey(string Name, string? Version);
}