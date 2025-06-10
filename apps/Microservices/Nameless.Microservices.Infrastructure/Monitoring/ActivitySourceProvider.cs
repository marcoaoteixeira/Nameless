using System.Collections.Concurrent;
using System.Diagnostics;
using Nameless.Microservices.Application.Monitoring;

namespace Nameless.Microservices.Infrastructure.Monitoring;

/// <summary>
///     Default implementation of <see cref="IActivitySourceProvider" />.
/// </summary>
public sealed class ActivitySourceProvider : IActivitySourceProvider {
    private readonly ConcurrentDictionary<CacheKey, ActivitySource> _cache = [];

    /// <inheritdoc />
    /// <remarks>
    ///     As one of the best practices mentioned <a href="https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs#best-practices-1">here</a>,
    ///     we cache the <see cref="ActivitySource"/> once it is requested.
    ///     We use the name and version parameters to create a cache key and when
    ///     it is requested a second time, these information will be used to
    ///     retrieve the <see cref="ActivitySource"/> instance.
    /// </remarks>
    public ActivitySource GetActivitySource(string name, string? version = null, IDictionary<string, object?>? tags = null) {
        var key = new CacheKey(name, version);

        return _cache.GetOrAdd(key, current => CreateActivitySource(current.Name, current.Version, tags));
    }

    private static ActivitySource CreateActivitySource(string name, string? version = null, IDictionary<string, object?>? tags = null) {
        return new ActivitySource(name, version ?? string.Empty, tags);
    }

    public readonly record struct CacheKey(string Name, string? Version);
}