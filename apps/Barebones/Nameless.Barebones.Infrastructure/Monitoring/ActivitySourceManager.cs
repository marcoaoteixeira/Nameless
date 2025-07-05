using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nameless.Barebones.Infrastructure.Monitoring;

/// <summary>
///     Default implementation of <see cref="IActivitySourceManager" />.
/// </summary>
public sealed class ActivitySourceManager : IActivitySourceManager, IDisposable {
    private readonly ConcurrentDictionary<CacheKey, ActivitySource> _cache = [];

    private bool _disposed;

    ~ActivitySourceManager() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Trying to follow the best practices mentioned <a href="https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-instrumentation-walkthroughs#best-practices-1">here</a>,
    ///     we cache the <see cref="ActivitySource"/> once it is requested.
    ///     We use the <paramref name="sourceName"/> and version parameters
    ///     to create a cache key and when it is requested a second time,
    ///     these information will be used to retrieve the
    ///     <see cref="ActivitySource"/> instance.
    /// </remarks>
    public ActivitySource GetActivitySource(string sourceName, string? version = null, IDictionary<string, object?>? tags = null) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(sourceName);

        var key = new CacheKey(sourceName, version);

        return GetActivitySource(key, tags);
    }

    /// <inheritdoc />
    public bool RemoveActiveSource(string sourceName, string? version = null) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(sourceName);

        var key = new CacheKey(sourceName, version);

        if (!_cache.TryRemove(key, out var activitySource)) {
            return false;
        }

        activitySource.Dispose();

        return true;
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var keys = _cache.Keys.ToArray();
            foreach (var key in keys) {
                if (_cache.TryRemove(key, out var activitySource)) {
                    activitySource.Dispose();
                }
            }
        }

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private ActivitySource GetActivitySource(CacheKey key, IDictionary<string, object?>? tags) {
        return _cache.GetOrAdd(key, current => new ActivitySource(current.Name, current.Version, tags));
    }

    public readonly record struct CacheKey(string Name, string? Version);
}