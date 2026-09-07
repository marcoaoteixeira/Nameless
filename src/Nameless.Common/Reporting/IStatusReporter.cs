namespace Nameless.Reporting;

/// <summary>
///     Provides way to write side of a service's status.
/// </summary>
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
/// <remarks>
///     Write side of a service's status. Only the service that owns this
///     status (e.g. the BackgroundService itself) should hold a reference
///     to this interface - everyone else should depend on
///     <see cref="IStatusMonitor{TService}"/>.
/// </remarks>
public interface IStatusReporter<TService> : IStatusReporter;

/// <summary>
///     Provides way to write side of a service's status.
/// </summary>
/// <remarks>
///     Write side of a service's status. Only the service that owns this
///     status (e.g. the BackgroundService itself) should hold a reference
///     to this interface - everyone else should depend on
///     <see cref="IStatusMonitor"/>.
/// </remarks>
public interface IStatusReporter {
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
    /// <param name="metadata">
    ///     The metadata.
    /// </param>
    void Report(string message, StatusLevel level, Dictionary<string, string>? metadata);

    /// <summary>
    ///     Signals that this channel has finished successfully. No further
    ///     <see cref="Report"/> calls will have any effect. Consumers observe
    ///     this as <see cref="IObserver{T}.OnCompleted"/>.
    /// </summary>
    void Complete();

    /// <summary>
    ///     Signals that this channel has finished with a failure. No further
    ///     <see cref="Report"/> calls will have any effect. Consumers observe
    ///     this as <see cref="IObserver{T}.OnError"/> with a
    ///     <see cref="FaultException"/>.
    /// </summary>
    /// <param name="reason">
    ///     A human-readable description of why the channel faulted.
    /// </param>
    /// <param name="code">
    ///     An optional machine-readable error code.
    /// </param>
    void Fault(string reason, string? code);

    /// <summary>
    ///     Signals that this channel has finished with a failure. No further
    ///     <see cref="Report"/> calls will have any effect. Consumers observe
    ///     this as <see cref="IObserver{T}.OnError"/> with a
    ///     <see cref="FaultException"/>.
    /// </summary>
    /// <param name="ex">
    ///     The current flow exception.
    /// </param>
    void Fault(Exception ex);
}