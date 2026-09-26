namespace Nameless.Reporting;

/// <summary>
///     Provides view of a service's current status.
/// </summary> 
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
/// <remarks>
///     Read-only view of a service's current status. This is what consumers
///     (UI, SignalR hubs, health checks, etc.) should depend on - it exposes
///     no way to push a new status, only to observe the latest one.
/// </remarks>
public interface IStatusMonitor<TService> : IStatusMonitor;

/// <summary>
///     Provides view of a service's current status.
/// </summary> 
/// <remarks>
///     Read-only view of a service's current status. This is what consumers
///     (UI, SignalR hubs, health checks, etc.) should depend on - it exposes
///     no way to push a new status, only to observe the latest one.
/// </remarks>
public interface IStatusMonitor {
    /// <summary>
    ///     Gets the name used to tag every <see cref="StatusUpdate"/> from
    ///     this source.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    ///     Gets the key that identifies this specific channel among the
    ///     possibly-many concurrent channels for the service (e.g. the file
    ///     name a file-processing worker instance is handling). Empty when
    ///     the service only ever has a single channel.
    /// </summary>
    string? ChannelKey { get; }

    /// <summary>
    ///     Gets the live status stream. Subscribing always immediately yields
    ///     the buffered recent values (up to the configured buffer size),
    ///     followed by any further live updates - or, once the channel has
    ///     reached a terminal state, the buffered history followed by the
    ///     terminal signal (<c>OnCompleted</c>/<c>OnError</c>).
    /// </summary>
    IObservable<StatusUpdate> Status { get; }
}