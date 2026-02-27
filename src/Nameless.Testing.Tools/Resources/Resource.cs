namespace Nameless.Testing.Tools.Resources;

public sealed class Resource : IDisposable, IAsyncDisposable {
    private Stream? _stream;
    private bool _disposed;

    public string Path { get; }

    public bool DeleteOnDispose { get; }

    public Resource(string path, bool deleteOnDispose) {
        Path = path;
        DeleteOnDispose = deleteOnDispose;
    }

    ~Resource() {
        Dispose(disposing: false);
    }

    public Stream Open() {
        BlockAccessAfterDispose();

        return _stream ??= DeleteOnDispose
            ? File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
            : File.Open(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public string GetContent() {
        BlockAccessAfterDispose();

        return File.Exists(Path)
            ? File.ReadAllText(Path)
            : string.Empty;
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

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _stream?.Dispose();
        }

        _stream = null;

        Destroy();

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore() {
        if (_stream is not null) {
            await _stream.DisposeAsync()
                         .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Destroy() {
        if (!DeleteOnDispose) { return; }

        try { if (File.Exists(Path)) { File.Delete(Path); } }
        catch { /* swallow */ }
    }
}
