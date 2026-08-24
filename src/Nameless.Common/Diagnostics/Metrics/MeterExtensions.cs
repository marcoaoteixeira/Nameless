namespace Nameless.Diagnostics.Metrics;

/// <summary>
///     <see cref="IMeter"/> extension methods.
/// </summary>
public static class MeterExtensions {
    /// <param name="self">
    ///     The current <see cref="IMeter"/> instance.
    /// </param>
    extension(IMeter self) {
        /// <summary>
        ///     Create a metrics StopwatchHistogram object.
        /// </summary>
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
        ///     An instance of <see cref="IDisposable"/>.
        /// </returns>
        public StopwatchHistogram CreateStopwatchHistogram(string name, string? unit = null, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null) {
            var histogram = self.CreateHistogram<long>(name, unit, description, tags);

            return new StopwatchHistogram(histogram);
        }
    }
}
