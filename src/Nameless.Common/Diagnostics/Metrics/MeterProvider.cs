using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Nameless.Diagnostics.Metrics;

/// <summary>
///     Represents a cache for instances of <see cref="Meter"/>.
/// </summary>
public static class MeterProvider {
    private static readonly ConcurrentDictionary<CacheKey, MeterWrapper> Cache = [];

    /// <summary>
    ///     Creates a new <see cref="IMeter"/> instance
    /// </summary>
    /// <param name="assembly">
    ///     The assembly to bind the meter; if not provided the
    ///     meter will be bind to the calling assembly.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IMeter"/>.
    /// </returns>
    public static IMeter Create(Assembly? assembly = null){
        var key = CacheKey.CreateKey(assembly ?? Assembly.GetCallingAssembly());

        return Cache.GetOrAdd(key, CreateMeter);
    }

    private static MeterWrapper CreateMeter(CacheKey key) {
        var meter = new Meter(key.Name, key.Version);
        var result = new MeterWrapper(meter);

        result.OnDispose += RemoveFromCache;

        return result;
    }

    private static void RemoveFromCache(IMeter meter) {
        var key = CacheKey.CreateKey(meter);

        if (Cache.TryRemove(key, out var output)) {
            output.OnDispose -= RemoveFromCache;
        }
    }
}