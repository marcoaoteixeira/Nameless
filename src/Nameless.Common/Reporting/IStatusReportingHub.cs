namespace Nameless.Reporting;

/// <summary>
///     Acts as a hub enabling easy retrieval of <see cref="IStatusMonitor"/>
///     or <see cref="IStatusReporter"/> for a given service channel.
/// </summary>
public interface IStatusReportingHub {
    /// <summary>
    ///     Returns the existing channel for the given
    ///     <paramref name="service"/> / <paramref name="channelKey"/> pair,
    ///     creating and registering a new one if none exists (or the previous
    ///     one was evicted after reaching a terminal state).
    /// </summary>
    /// <param name="service">
    ///     The type this hub's channels belong to (e.g. a specific
    ///     BackgroundService). Used purely as a discriminator, not constructed.
    /// </param>
    /// <param name="channelKey">
    ///     An optional key identifying a named sub-channel (e.g. a file name,
    ///     a Kafka topic). Pass <see langword="null"/> for the default channel.
    /// </param>
    /// <returns>
    ///     The write-side reporter for that channel.
    /// </returns>
    IStatusReporter GetReporter(Type service, string? channelKey = null);

    /// <summary>
    ///     Returns the existing channel for the given
    ///     <paramref name="service"/> / <paramref name="channelKey"/> pair,
    ///     creating and registering a new one if none exists (or the previous
    ///     one was evicted after reaching a terminal state).
    /// </summary>
    /// <param name="service">
    ///     The type this hub's channels belong to (e.g. a specific
    ///     BackgroundService). Used purely as a discriminator, not constructed.
    /// </param>
    /// <param name="channelKey">
    ///     An optional key identifying a named sub-channel. Pass
    ///     <see langword="null"/> for the default channel.
    /// </param>
    /// <returns>
    ///     The read-side monitor for that channel.
    /// </returns>
    IStatusMonitor GetMonitor(Type service, string? channelKey = null);
}
