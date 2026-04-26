using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Nameless.Diagnostics;

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
        var activitySource = new ActivitySource(key.Name, key.Version);
        var result = new ActivitySourceWrapper(activitySource);

        result.OnDispose += RemoveFromCache;

        return result;
    }

    private static void RemoveFromCache(IActivitySource activitySource) {
        var key = CacheKey.CreateKey(activitySource);

        if (Cache.TryRemove(key, out var output)) {
            output.OnDispose -= RemoveFromCache;
        }
    }

    internal readonly record struct CacheKey(string Name, string Version) {
        internal static CacheKey CreateKey(Assembly assembly) {
            return new CacheKey {
                Name = assembly.GetSemanticName(),
                Version = assembly.GetSemanticVersion(prefix: "v")
            };
        }

        internal static CacheKey CreateKey(IActivitySource activitySource) {
            return new CacheKey {
                Name = activitySource.Name,
                Version = activitySource.Version ?? string.Empty
            };
        }
    }
}
