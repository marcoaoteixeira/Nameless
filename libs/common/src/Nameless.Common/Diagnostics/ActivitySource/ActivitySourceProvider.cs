using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Nameless.Diagnostics.ActivitySource;

/// <summary>
///     Represents a cache for instances of <see cref="ActivitySource"/>.
/// </summary>
public static class ActivitySourceProvider {
    private static readonly ConcurrentDictionary<CacheKey, ActivitySourceWrapper> Cache = [];

    /// <summary>
    ///     Creates a new <see cref="IActivitySource"/> instance
    /// </summary>
    /// <param name="assembly">
    ///     The assembly to bind the activity source; if not provided the
    ///     activity source will be bind to the calling assembly.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IActivitySource"/>.
    /// </returns>
    public static IActivitySource Create(Assembly? assembly = null) {
        var key = CacheKey.CreateKey(assembly ?? Assembly.GetCallingAssembly());

        return Cache.GetOrAdd(key, CreateActivitySource);
    }

    private static ActivitySourceWrapper CreateActivitySource(CacheKey key) {
        var activitySource = new System.Diagnostics.ActivitySource(key.Name, key.Version);
        var result = new ActivitySourceWrapper(activitySource);

        result.OnDispose += RemoveFromCache;

        return result;
    }

    private static void RemoveFromCache(IActivitySource activitySource) {
        var key = CacheKey.CreateKey(
            activitySource
        );

        if (Cache.TryRemove(key, out var output)) {
            output.OnDispose -= RemoveFromCache;
        }
    }
}