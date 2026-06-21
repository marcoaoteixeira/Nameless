using Nameless.Null;

namespace Nameless.Workers.Notification;

/// <summary>
///     A thread-safe, lock-based implementation of
///     <see cref="IObservable{T}" /> for <see cref="WorkerProgress" />
///     notifications. Used internally by <see cref="Worker" />.
/// </summary>
internal sealed class WorkerProgressSubject : IObservable<WorkerProgress>, IDisposable {
    private readonly List<IObserver<WorkerProgress>> _observers = [];
    private readonly Lock _gate = new();

    private bool _terminated;
    private bool _disposed;

    /// <summary>
    ///     Gets the most recently published <see cref="WorkerProgress" />,
    ///     or <see langword="null" /> if no notification has been published yet.
    ///     Reads are lock-free; a stale read is acceptable for poll scenarios.
    /// </summary>
    internal WorkerProgress? LastProgress { get; private set; }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<WorkerProgress> observer) {
        lock (_gate) {
            if (_disposed || _terminated) {
                observer.OnCompleted();
                return NullDisposable.Instance;
            }

            _observers.Add(observer);
        }

        return new Unsubscriber(this, observer);
    }

    /// <summary>
    ///     Publishes <paramref name="value" /> to all current subscribers and
    ///     updates <see cref="LastProgress" />.
    /// </summary>
    internal void OnNext(WorkerProgress value) {
        IObserver<WorkerProgress>[] snapshot;

        lock (_gate) {
            if (_disposed || _terminated) { return; }

            LastProgress = value;

            snapshot = [.. _observers];
        }

        foreach (var observer in snapshot) {
            try { observer.OnNext(value); }
            catch { /* observer is responsible for its own error handling */ }
        }
    }

    /// <summary>
    ///     Signals completion to all subscribers and clears the subscriber list.
    ///     Subsequent calls are no-ops.
    /// </summary>
    internal void OnCompleted() {
        IObserver<WorkerProgress>[] snapshot;

        lock (_gate) {
            if (_disposed || _terminated) { return; }

            _terminated = true;

            snapshot = [.. _observers];
            
            _observers.Clear();
        }

        foreach (var observer in snapshot) {
            try { observer.OnCompleted(); }
            catch { /* observer is responsible for its own error handling */ }
        }
    }

    /// <summary>
    ///     Signals an error to all subscribers and clears the subscriber list.
    /// </summary>
    internal void OnError(Exception error) {
        IObserver<WorkerProgress>[] snapshot;

        lock (_gate) {
            if (_disposed || _terminated) { return; }

            _terminated = true;

            snapshot = [.. _observers];
            
            _observers.Clear();
        }

        foreach (var observer in snapshot) {
            try { observer.OnError(error); }
            catch { /* observer is responsible for its own error handling */ }
        }
    }

    private void Unsubscribe(IObserver<WorkerProgress> observer) {
        lock (_gate) {
            _observers.Remove(observer);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        OnCompleted();

        lock (_gate) {
            _disposed = true;
        }
    }

    private sealed class Unsubscriber(WorkerProgressSubject subject, IObserver<WorkerProgress> observer) : IDisposable {
        private bool _disposed;

        public void Dispose() {
            if (_disposed) { return; }

            _disposed = true;
            
            subject.Unsubscribe(observer);
        }
    }
}
