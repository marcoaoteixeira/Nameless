namespace Nameless.Web.Infrastructure;

public sealed class PeriodicTimerWrapper : IPeriodicTimer {
    private PeriodicTimer _timer;

    private bool _disposed;

    public TimeSpan Period {
        get => GetPeriod();
        set => SetPeriod(value);
    }

    public PeriodicTimerWrapper(TimeSpan interval) {
        Guard.Against.LowerOrEqual(interval, TimeSpan.Zero);

        _timer = new PeriodicTimer(interval);
    }

    ~PeriodicTimerWrapper() {
        Dispose(disposing: false);
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        return _timer.WaitForNextTickAsync(cancellationToken);
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _timer.Dispose();
        }

        _timer = null!; // Prevent further access to the disposed timer
        _disposed = true;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private TimeSpan GetPeriod() {
        BlockAccessAfterDispose();

        return _timer.Period;
    }

    private void SetPeriod(TimeSpan value) {
        BlockAccessAfterDispose();

        Guard.Against.LowerOrEqual(value, TimeSpan.Zero);

        _timer.Period = value;
    }
}
