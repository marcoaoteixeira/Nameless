using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Nameless.Diagnostics;

/// <summary>
///     Represents a cache for instances of <see cref="ActivitySource"/>.
/// </summary>
public static class ActivitySourceProvider {
    private static ConcurrentDictionary<CacheKey, ActivitySourceWrapper> Cache { get; } = [];
    
    /// <summary>
    ///     Creates a new <see cref="IActivitySource"/> instance
    /// </summary>
    /// <returns>
    ///     A new instance of <see cref="IActivitySource"/>.
    /// </returns>
    public static IActivitySource Create() {
        var key = CacheKey.Create(Assembly.GetCallingAssembly());

        return Cache.GetOrAdd(key, CreateActivitySource);
    }

    private static ActivitySourceWrapper CreateActivitySource(CacheKey key) {
        var result = new ActivitySourceWrapper(
            new ActivitySource(key.Name, key.Version)
        );

        result.OnDispose += RemoveFromCache;

        return result;
    }

    private static void RemoveFromCache(IActivitySource activitySource) {
        var key = CacheKey.Create(activitySource);

        if (Cache.TryRemove(key, out var output)) {
            output.OnDispose -= RemoveFromCache;
        }
    }

    internal readonly record struct CacheKey(string Name, string Version) {
        internal static CacheKey Create(Assembly assembly) {
            return new CacheKey {
                Name = assembly.GetSemanticName(),
                Version = assembly.GetSemanticVersion()
            };
        }

        internal static CacheKey Create(IActivitySource activitySource) {
            return new CacheKey {
                Name = activitySource.Name,
                Version = activitySource.Version ?? string.Empty
            };
        }
    }
}
