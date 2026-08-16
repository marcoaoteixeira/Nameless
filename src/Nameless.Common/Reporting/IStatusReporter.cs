namespace Nameless.Reporting;

/// <summary>
///     Write side of a service's status. Only the service that owns this
///     status (e.g. the BackgroundService itself) should hold a reference
///     to this interface - everyone else should depend on
///     <see cref="IStatusMonitor{TService}"/>.
/// </summary>
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
public interface IStatusReporter<TService> : IStatusMonitor<TService> {
    /// <summary>
    ///     Publishes a new current status, replacing whatever was reported
    ///     before.
    /// </summary>
    /// <param name="message">
    ///     The reporting message.
    /// </param>
    /// <param name="level">
    ///     The status level.
    /// </param>
    void Report(string message, StatusLevel level);
}