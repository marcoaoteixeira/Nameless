namespace Nameless.Testing.Tools.Mockers.EntityFrameworkCore;

public class InMemoryAsyncEnumerator<T> : IAsyncEnumerator<T>, IDisposable {
    private readonly IEnumerator<T> _enumerator;
    private bool _disposed;

    public T Current => _enumerator.Current;

    public InMemoryAsyncEnumerator(IEnumerator<T> enumerator) {
        _enumerator = enumerator;
    }

    ~InMemoryAsyncEnumerator() {
        Dispose(disposing: false);
    }

    public ValueTask<bool> MoveNextAsync() {
        return ValueTask.FromResult(_enumerator.MoveNext());
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsyncCore().ConfigureAwait(continueOnCapturedContext: false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _enumerator.Dispose();
        }

        _disposed = true;
    }

    protected virtual ValueTask DisposeAsyncCore() {
        return ValueTask.CompletedTask;
    }
}
