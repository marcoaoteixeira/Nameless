namespace Nameless.Reporting;

/// <summary>
///     Provides means to retrieve the <see cref="IStatusMonitor{TService}"/>.
/// </summary>
/// <remarks>
///     Read side of a keyed registry of status channels. Consumers use this
///     to look up a specific channel by key without being able to create or
///     write to it.
/// </remarks>
public interface IStatusMonitorHub {
    /// <summary>
    ///     Attempts to find the channel registered for
    ///     <paramref name="channelKey"/>. Does not create one - a missing
    ///     key means either it was never reported to, or its channel has
    ///     already reached a terminal state and been evicted.
    /// </summary>
    /// <typeparam name="TService">
    ///     The type this hub's channels belong to (e.g. a specific
    ///     BackgroundService). Used purely as a compile-time-safe discriminator,
    ///     not constructed.
    /// </typeparam>
    /// <param name="channelKey">
    ///     The key identifying the channel.
    /// </param>
    /// <param name="monitor">
    ///     The monitor for that channel, when found.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the channel was found; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    bool TryGet<TService>(string channelKey, out IStatusMonitor<TService>? monitor);
}
