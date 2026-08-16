using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Nameless.Diagnostics.Metrics;

/// <summary>
///     Stopwatch + Meter
/// </summary>
public sealed class StopwatchHistogram : IDisposable {
    private readonly Histogram<long> _histogram;

    private Stopwatch? _sw;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of <see cref="StopwatchHistogram"/> class.
    /// </summary>
    /// <param name="histogram">
    ///     The <see cref="Histogram{T}"/> instrument associated with
    ///     the <see cref="StopwatchHistogram"/>.
    /// </param>
    public StopwatchHistogram(Histogram<long> histogram) {
        _histogram = histogram;
        _sw = Stopwatch.StartNew();
    }

    /// <summary>
    ///     Records the current moment with a tag.
    /// </summary>
    /// <param name="tagName">
    ///     The tag name.
    /// </param>
    /// <param name="tagValue">
    ///     The tag value.
    /// </param>
    public void Mark(string tagName, object? tagValue = null) {
        BlockAccessAfterDispose();

        Throws.When.NullOrWhiteSpace(tagName);

        _histogram.Record(
            _sw?.ElapsedMilliseconds ?? 0L,
            new KeyValuePair<string, object?>(tagName, tagValue)
        );
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~StopwatchHistogram() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public void Dispose() {
        GC.SuppressFinalize(this);
        Dispose(disposing: true);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }
        if (disposing) { }

        _histogram.Record(_sw?.ElapsedMilliseconds ?? 0L);

        _sw = null;

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}
