using System.Diagnostics.Metrics;

namespace Nameless.Diagnostics.Metrics;

/// <summary>
///     Exposes the basics of <see cref="Meter"/>.
/// </summary>
public interface IMeter : IDisposable {
    /// <summary>
    ///     Gets the meter name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the meter version.
    /// </summary>
    string? Version { get; }

    /// <summary>
    ///     Histogram is an Instrument which can be used to report arbitrary
    ///     values that are likely to be statistically meaningful. It is
    ///     intended for statistics such as histograms, summaries, and
    ///     percentile.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the instrument report value.
    /// </typeparam>
    /// <param name="name">
    ///     The instrument name. Cannot be null.
    /// </param>
    /// <param name="unit">
    ///     Optional instrument unit of measurements.
    /// </param>
    /// <param name="description">
    ///     Optional instrument description.
    /// </param>
    /// <param name="tags">
    ///     Optional tags to attach to the instrument.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="Histogram{T}"/>.
    /// </returns>
    Histogram<T> CreateHistogram<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct;

    /// <summary>
    ///     Creates a Gauge instrument, which can be used to record non-additive values.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the instrument report value.
    /// </typeparam>
    /// <param name="name">
    ///     The instrument name. Cannot be null.
    /// </param>
    /// <param name="unit">
    ///     Optional instrument unit of measurements.
    /// </param>
    /// <param name="description">
    ///     Optional instrument description.
    /// </param>
    /// <param name="tags">
    ///     Optional tags to attach to the instrument.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="Gauge{T}"/>.
    /// </returns>
    Gauge<T> CreateGauge<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct;

    /// <summary>
    ///     Create a metrics Counter object.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the instrument report value.
    /// </typeparam>
    /// <param name="name">
    ///     The instrument name. Cannot be null.
    /// </param>
    /// <param name="unit">
    ///     Optional instrument unit of measurements.
    /// </param>
    /// <param name="description">
    ///     Optional instrument description.
    /// </param>
    /// <param name="tags">
    ///     Optional tags to attach to the instrument.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="Counter{T}"/>.
    /// </returns>
    Counter<T> CreateCounter<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct;

    /// <summary>
    ///     Create a metrics UpDownCounter object.
    /// </summary>
    /// <typeparam name="T">
    ///     Type of the instrument report value.
    /// </typeparam>
    /// <param name="name">
    ///     The instrument name. Cannot be null.
    /// </param>
    /// <param name="unit">
    ///     Optional instrument unit of measurements.
    /// </param>
    /// <param name="description">
    ///     Optional instrument description.
    /// </param>
    /// <param name="tags">
    ///     Optional tags to attach to the instrument.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="UpDownCounter{T}"/>.
    /// </returns>
    UpDownCounter<T> CreateUpDownCounter<T>(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) where T : struct;
}