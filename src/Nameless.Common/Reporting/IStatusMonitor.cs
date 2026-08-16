namespace Nameless.Reporting;

/// <summary>
///     Read-only view of a service's current status. This is what consumers
///     (UI, SignalR hubs, health checks, etc.) should depend on - it exposes
///     no way to push a new status, only to observe the latest one.
/// </summary>
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
public interface IStatusMonitor<TService> {
    /// <summary>
    ///     Gets the name used to tag every <see cref="StatusUpdate"/> from
    ///     this source.
    /// </summary>
    string ServiceName { get; }

    /// <summary>
    ///     Gets the live status stream. Subscribing always immediately yields
    ///     the most recently reported value (or an initial "Idle" value if
    ///     nothing has been reported yet) - no history beyond that single
    ///     latest value.
    /// </summary>
    IObservable<StatusUpdate> Status { get; }
}