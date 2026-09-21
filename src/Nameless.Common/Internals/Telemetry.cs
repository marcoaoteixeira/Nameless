using Nameless.Diagnostics.ActivitySource;
using Nameless.Diagnostics.Metrics;

namespace Nameless;

internal static class Telemetry {
    internal static IActivitySource ActivitySource { get; } = ActivitySourceProvider.Create(typeof(Telemetry).Assembly);
    
    internal static IMeter Meter { get; } = MeterProvider.Create(typeof(Telemetry).Assembly);
}
