using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace Nameless.Reporting;

/// <summary>
///     Default implementation of <see cref="IStatusReporter{TService}"/>,
///     backed by a <see cref="ReplaySubject{T}"/> so that every subscriber -
///     no matter when it subscribes - immediately receives the recent
///     buffered history (up to the configured buffer size), followed by any
///     further live updates or, once terminated, the terminal signal.
/// </summary>
/// <remarks>
///     This type is public (rather than internal) so it can be unit-tested
///     and constructed directly where useful, but it is intentionally NOT
///     the type application code should depend on. Use
///     <see cref="ServiceCollectionExtensions.RegisterStatusReporting{TService}"/>
///     to register it, which seals off unkeyed resolution of this concrete
///     type and only exposes it through
///     <see cref="IStatusReporter{TService}"/> /
///     <see cref="IStatusMonitor{TService}"/>.
/// </remarks>
public sealed class StatusReporter<TService> : IStatusReporter<TService>, IDisposable {
    private readonly ReplaySubject<StatusUpdate> _subject;
    private int _subscriberCount;
    private volatile bool _terminated;

    /// <inheritdoc />
    public string ServiceName { get; }

    /// <inheritdoc />
    public string ChannelKey { get; }

    /// <summary>
    ///     Fires once the subscriber count drops to zero <em>after</em> this
    ///     channel has reached a terminal state. A hosting status reporter
    ///     hub listens to this to know when a finished channel can be
    ///     evicted from its registry.
    /// </summary>
    internal event Action? Idle;

    /// <inheritdoc />
    /// <remarks>
    ///     Deliberately NOT implemented via Rx's <c>Observable.Create</c>:
    ///     that operator wraps every subscriber in an internal
    ///     AutoDetachObserver that disposes the subscription resource the
    ///     instant OnCompleted/OnError is observed - which would decrement
    ///     the subscriber count before a consumer gets a chance to react to
    ///     the terminal notification, defeating refcounted eviction (used by
    ///     a hosting hub to know when a finished channel is safe to evict).
    ///     Implementing <see cref="IObservable{T}"/> directly keeps that
    ///     decrement tied solely to the consumer's own
    ///     <see cref="IDisposable.Dispose"/> call.
    /// </remarks>
    public IObservable<StatusUpdate> Status { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="StatusReporter{TService}"/> class.
    /// </summary>
    /// <param name="channelKey">
    ///     The key identifying this specific channel among the possibly-many
    ///     concurrent channels for <typeparamref name="TService"/>. Leave
    ///     empty when the service only ever has a single channel.
    /// </param>
    /// <param name="bufferSize">
    ///     The number of most-recent status updates replayed to new
    ///     subscribers.
    /// </param>
    public StatusReporter(string? channelKey = null, int bufferSize = 10) {
        if (string.IsNullOrWhiteSpace(channelKey)) {
            channelKey = string.Empty;
        }

        ChannelKey = channelKey;
        ServiceName = channelKey.Length == 0
            ? typeof(TService).Name
            : $"[{typeof(TService).Name}] {channelKey}";

        _subject = new ReplaySubject<StatusUpdate>(bufferSize);
        _subject.OnNext(
            new StatusUpdate(ServiceName, "Idle", StatusLevel.Info, DateTimeOffset.UtcNow)
        );

        Status = new ChannelObservable(this);
    }

    /// <inheritdoc />
    public void Report(string message, StatusLevel level = StatusLevel.Info) {
        BlockAccessAfterDispose();

        if (_terminated) {
            return;
        }

        _subject.OnNext(
            new StatusUpdate(ServiceName, message, level, DateTimeOffset.UtcNow)
        );
    }

    /// <inheritdoc />
    public void Complete() {
        BlockAccessAfterDispose();

        _terminated = true;
        _subject.OnCompleted();

        if (Volatile.Read(ref _subscriberCount) == 0) {
            Idle?.Invoke();
        }
    }

    /// <inheritdoc />
    public void Fault(string reason, string? code = null) {
        Fault(new FaultException(reason, code));
    }

    /// <inheritdoc />
    public void Fault(Exception ex) {
        BlockAccessAfterDispose();

        _terminated = true;
        _subject.OnError(ex);

        if (Volatile.Read(ref _subscriberCount) == 0) {
            Idle?.Invoke();
        }
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

    private sealed class ChannelObservable(StatusReporter<TService> owner) : IObservable<StatusUpdate> {
        public IDisposable Subscribe(IObserver<StatusUpdate> observer) {
            Interlocked.Increment(ref owner._subscriberCount);

            var subscription = owner._subject.Subscribe(observer);

            return Disposable.Create(() => {
                subscription.Dispose();

                if (Interlocked.Decrement(ref owner._subscriberCount) == 0 && owner._terminated) {
                    owner.Idle?.Invoke();
                }
            });
        }
    }
}
