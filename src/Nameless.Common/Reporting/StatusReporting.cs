using System.Reactive.Disposables;
using System.Reactive.Subjects;
using Microsoft.Extensions.Options;

namespace Nameless.Reporting;

/// <summary>
///     Aggregate <see cref="IStatusReporter"/> and <see cref="IStatusMonitor"/>
///     into a single class.
/// </summary>
public sealed class StatusReporting : IStatusReporter, IStatusMonitor, IDisposable {
    private readonly TimeProvider _timeProvider;
    private readonly ReplaySubject<StatusUpdate> _subject;

    private int _subscriberCount;
    private volatile bool _terminated;
    private bool _disposed;

    /// <inheritdoc />
    public string ServiceName { get; }

    /// <inheritdoc />
    public string? ChannelKey { get; }

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
    ///     Fires once the subscriber count drops to zero <em>after</em> this
    ///     channel has reached a terminal state. A hosting status reporter
    ///     hub listens to this to know when a finished channel can be
    ///     evicted from its registry.
    /// </summary>
    internal event Action? Idle;

    /// <summary>
    ///     Initializes a new instance of <see cref="StatusReporting"/> class.
    /// </summary>
    /// <param name="serviceName">
    ///     The service name.
    /// </param>
    /// <param name="channelKey">
    ///     The channel key.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    public StatusReporting(string serviceName, string? channelKey, TimeProvider timeProvider, IOptions<StatusReportingOptions> options) {
        _timeProvider = timeProvider;

        ServiceName = Throws.When.NullOrWhiteSpace(serviceName);
        ChannelKey = channelKey;

        _subject = new ReplaySubject<StatusUpdate>(options.Validate().Value.BufferSize);
        _subject.OnNext(
            new StatusUpdate(
                ServiceName,
                message: "Idle",
                ChannelKey,
                timestamp: _timeProvider.GetUtcNow()
            )
        );

        Status = new ChannelObservable(this);
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~StatusReporting() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public void Report(string message, StatusLevel level, Dictionary<string, string>? metadata) {
        BlockAccessAfterDispose();

        if (_terminated) { return; }

        _subject.OnNext(
            new StatusUpdate(
                ServiceName,
                message,
                ChannelKey,
                level,
                _timeProvider.GetUtcNow(),
                metadata
            )
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
    public void Fault(string reason, string? code) {
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

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _subject.Dispose();
        }

        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
    
    private sealed class ChannelObservable(StatusReporting owner) : IObservable<StatusUpdate> {
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