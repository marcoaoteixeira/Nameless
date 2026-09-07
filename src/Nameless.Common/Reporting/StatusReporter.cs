namespace Nameless.Reporting;

/// <summary>
///     Generic implementation of <see cref="IStatusReporter"/>.
/// </summary>
/// <typeparam name="TService">
///     The type this status stream belongs to (e.g. a specific
///     BackgroundService). Used purely as a compile-time-safe discriminator,
///     not constructed.
/// </typeparam>
public sealed class StatusReporter<TService> : IStatusReporter<TService>, IDisposable {
    private IStatusReporter Reporter { get; }

    private int ReporterHashCode => Reporter.GetHashCode();

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="StatusReporter{TService}"/> class.
    /// </summary>
    /// <param name="hub">
    ///     The hub.
    /// </param>
    public StatusReporter(IStatusReportingHub hub) {
        Reporter = hub.GetReporter(typeof(TService), channelKey: null);
    }

    /// <inheritdoc />
    public void Report(string message, StatusLevel level, Dictionary<string, string>? metadata) {
        Reporter.Report(message, level, metadata);
    }

    /// <inheritdoc />
    public void Complete() {
        Reporter.Complete();
    }

    /// <inheritdoc />
    public void Fault(string reason, string? code = null) {
        Reporter.Fault(reason, code);
    }

    /// <inheritdoc />
    public void Fault(Exception ex) {
        Reporter.Fault(ex);
    }

    /// <inheritdoc />
    public void Dispose() {
        if (Reporter is IDisposable disposable) {
            disposable.Dispose();
        }
    }
}