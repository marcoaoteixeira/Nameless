namespace Nameless.Reporting;

/// <summary>
///     Generic implementation of <see cref="IStatusMonitor"/>.
/// </summary>
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
public sealed class StatusMonitor<TService> : IStatusMonitor<TService> {
    private IStatusMonitor Monitor { get; }

    private int MonitorHashCode => Monitor.GetHashCode();

    /// <inheritdoc />
    public string ServiceName => Monitor.ServiceName;

    /// <inheritdoc />
    public string? ChannelKey => Monitor.ChannelKey;

    /// <inheritdoc />
    public IObservable<StatusUpdate> Status => Monitor.Status;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="StatusMonitor{TService}"/> class.
    /// </summary>
    /// <param name="hub">
    ///     The hub.
    /// </param>
    public StatusMonitor(IStatusReportingHub hub) {
        Monitor = hub.GetMonitor(typeof(TService), channelKey: null);
    }
}
