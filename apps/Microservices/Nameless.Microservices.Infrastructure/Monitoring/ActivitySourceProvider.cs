using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nameless.Microservices.Infrastructure.Monitoring;

/// <summary>
///     Default implementation of <see cref="IActivitySourceProvider" />.
/// </summary>
public sealed class ActivitySourceProvider : IActivitySourceProvider {
    private readonly ConcurrentDictionary<string, ActivitySource> _cache = [];

    /// <inheritdoc />
    public ActivitySource GetActivitySource(string name, string? version = null, IDictionary<string, object?>? tags = null) {
        return _cache.GetOrAdd(name, key => CreateActivitySource(key, version, tags));
    }

    private static ActivitySource CreateActivitySource(string name, string? version = null, IDictionary<string, object?>? tags = null) {
        return new ActivitySource(name, version ?? string.Empty, tags);
    }
}