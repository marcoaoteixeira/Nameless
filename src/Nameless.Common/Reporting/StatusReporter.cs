using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Nameless.Reporting;

/// <summary>
///     Default implementation of <see cref="IStatusReporter{TService}"/>,
///     backed by a <see cref="BehaviorSubject{T}"/> so that every subscriber
///     - no matter when it subscribes - immediately receives the most
///     recently reported value, with no persisted history beyond that.
/// </summary>
/// <remarks>
///     This type is public (rather than internal) so it can be unit-tested
///     and constructed directly where useful, but it is intentionally NOT
///     the type application code should depend on. Use
///     <see cref="ServiceCollectionExtensions.RegisterStatusReporter{TService}"/>
///     to register it, which seals off unkeyed resolution of this concrete
///     type and only exposes it through
///     <see cref="IStatusReporter{TService}"/> /
///     <see cref="IStatusMonitor{TService}"/>.
/// </remarks>
public sealed class StatusReporter<TService> : IStatusReporter<TService>, IDisposable {
    private readonly BehaviorSubject<StatusUpdate> _subject;

    /// <inheritdoc />
    public string ServiceName { get; }

    /// <inheritdoc />
    public IObservable<StatusUpdate> Status => _subject.AsObservable();

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="StatusReporter{TService}"/> class.
    /// </summary>
    public StatusReporter() {
        ServiceName = typeof(TService).Name;

        _subject = new BehaviorSubject<StatusUpdate>(
            new StatusUpdate(ServiceName, "Idle", StatusLevel.Info, DateTimeOffset.UtcNow)
        );
    }

    /// <inheritdoc />
    public void Report(string message, StatusLevel level = StatusLevel.Info) {
        BlockAccessAfterDispose();

        _subject.OnNext(
            new StatusUpdate(ServiceName, message, level, DateTimeOffset.UtcNow)
        );
    }

    /// <summary>
    ///     Completes the underlying subject. Safe to call on host shutdown;
    ///     not required for correctness of Report/Status while the process
    ///     runs.
    /// </summary>
    public void Dispose() {
        _subject.Dispose();
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_subject.IsDisposed, this);
    }
}