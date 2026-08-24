namespace Nameless.Reporting;

/// <summary>
///     Provides way to get or create <see cref="IStatusReporter{TService}"/>
///     services.
/// </summary>
/// <remarks>
///     Write side of a keyed registry of status channels. Only the component
///     that owns channel creation (e.g. a factory spinning up one channel
///     per file/topic/job) should depend on this interface - everyone else
///     should depend on <see cref="IStatusMonitorHub"/>.
/// </remarks>
public interface IStatusReporterHub {
    /// <summary>
    ///     Gets the existing channel for <paramref name="channelKey"/>, or
    ///     creates and registers a brand-new one if none exists (or the
    ///     previous one for that key has since been evicted).
    /// </summary>
    /// <typeparam name="TService">
    ///     The type this hub's channels belong to (e.g. a specific
    ///     BackgroundService). Used purely as a compile-time-safe discriminator,
    ///     not constructed.
    /// </typeparam>
    /// <param name="channelKey">
    ///     The key identifying the channel (e.g. a file name, a Kafka topic).
    /// </param>
    /// <returns>
    ///     The reporter for that channel.
    /// </returns>
    IStatusReporter<TService> GetOrCreate<TService>(string channelKey);
}
