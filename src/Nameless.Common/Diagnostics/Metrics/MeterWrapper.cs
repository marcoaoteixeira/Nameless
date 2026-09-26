using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Nameless.Diagnostics.Metrics;

/// <summary>
///     <see cref="Meter"/> wrapper.
/// </summary>
public sealed class MeterWrapper : IMeter {
    private static readonly ConcurrentDictionary<string, Instrument> Cache = new(StringComparer.OrdinalIgnoreCase);

    private readonly Meter _meter;

    /// <summary>
    ///     The events that is triggered when Dispose occurs.
    /// </summary>
    public event Action<IMeter>? OnDispose;

    /// <inheritdoc />
    public string Name => _meter.Name;

    /// <inheritdoc />
    public string? Version => _meter.Version;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new <see cref="MeterWrapper"/> class.
    /// </summary>
    /// <param name="meter">
    ///     The internal meter.
    /// </param>
    public MeterWrapper(Meter meter) {
        _meter = meter;
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~MeterWrapper() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public Histogram<T> CreateHistogram<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct {
        BlockAccessAfterDispose();

        Throws.When.NullOrWhiteSpace(name);

        var instrument = Cache.GetOrAdd($"HISTOGRAM::{name}", _ => _meter.CreateHistogram<T>(name, unit, description, tags));

        return (Histogram<T>)instrument;
    }

    /// <inheritdoc />
    public Gauge<T> CreateGauge<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct {
        BlockAccessAfterDispose();

        Throws.When.NullOrWhiteSpace(name);

        var instrument = Cache.GetOrAdd($"GAUGE::{name}", _ => _meter.CreateGauge<T>(name, unit, description, tags));

        return (Gauge<T>)instrument;
    }

    /// <inheritdoc />
    public Counter<T> CreateCounter<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct {
        BlockAccessAfterDispose();

        Throws.When.NullOrWhiteSpace(name);

        var instrument = Cache.GetOrAdd($"COUNTER::{name}", _ => _meter.CreateCounter<T>(name, unit, description, tags ?? []));

        return (Counter<T>)instrument;
    }

    /// <inheritdoc />
    public UpDownCounter<T> CreateUpDownCounter<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct {
        BlockAccessAfterDispose();

        Throws.When.NullOrWhiteSpace(name);

        var instrument = Cache.GetOrAdd($"UPDOWNCOUNTER::{name}", _ => _meter.CreateUpDownCounter<T>(name, unit, description, tags ?? []));

        return (UpDownCounter<T>)instrument;
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _meter.Dispose();

            lock (Cache) { Cache.Clear(); }
        }

        OnDispose?.Invoke(this);

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}