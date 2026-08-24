using System.Diagnostics;
using System.Diagnostics.Metrics;
using Nameless.Diagnostics.ActivitySource;
using Nameless.Diagnostics.Metrics;

namespace Nameless;

internal static class DiagnosticsHelper {
    private static readonly IActivitySource ActivitySource = ActivitySourceProvider.Create(typeof(DiagnosticsHelper).Assembly);
    private static readonly IMeter Meter = MeterProvider.Create(typeof(DiagnosticsHelper).Assembly);

    internal static IActivity StartActivity(string name, ActivityKind kind = ActivityKind.Internal, ActivityContext? parentContext = null) {
        return ActivitySource.StartActivity(name, kind, parentContext);
    }

    internal static Counter<T> CreateCounter<T>(string name, string? description = null, string? unit = null, Dictionary<string, object?>? tags = null) where T : struct {
        return Meter.CreateCounter<T>(name, unit, description, tags: [..tags ?? []]);
    }

    internal static UpDownCounter<T> UpDownCreateCounter<T>(string name, string? description = null, string? unit = null, Dictionary<string, object?>? tags = null) where T : struct {
        return Meter.CreateUpDownCounter<T>(name, unit, description, tags: [..tags ?? []]);
    }

    internal static Histogram<T> CreateHistogram<T>(string name, string? description = null, string? unit = null, Dictionary<string, object?>? tags = null) where T : struct {
        return Meter.CreateHistogram<T>(name, unit, description, tags: [.. tags ?? []]);
    }

    internal static StopwatchHistogram CreateStopwatchHistogram(string name, string? description = null, string? unit = null, Dictionary<string, object?>? tags = null) {
        return Meter.CreateStopwatchHistogram(name, unit, description, tags: [.. tags ?? []]);
    }

    internal static Gauge<T> CreateGauge<T>(string name, string? description = null, string? unit = null, Dictionary<string, object?>? tags = null) where T : struct {
        return Meter.CreateGauge<T>(name, unit, description, tags: [.. tags ?? []]);
    }
}
