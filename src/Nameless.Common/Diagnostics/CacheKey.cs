using System.Reflection;
using Nameless.Diagnostics.ActivitySource;
using Nameless.Diagnostics.Metrics;

namespace Nameless.Diagnostics;

internal readonly record struct CacheKey(string Name, string Version) {
    internal static CacheKey CreateKey(Assembly assembly) {
        return new CacheKey {
            Name = assembly.GetSemanticName(),
            Version = assembly.GetSemanticVersion(prefix: 'v')
        };
    }

    internal static CacheKey CreateKey(IActivitySource activitySource) {
        return new CacheKey {
            Name = activitySource.Name,
            Version = activitySource.Version ?? string.Empty
        };
    }

    internal static CacheKey CreateKey(IMeter meter) {
        return new CacheKey {
            Name = meter.Name,
            Version = meter.Version ?? string.Empty
        };
    }
}