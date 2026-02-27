using System.Diagnostics;

namespace Nameless.Diagnostics;

/// <summary>
///     A wrapper around <see cref="Activity"/> that implements
///     <see cref="IActivity"/>.
/// </summary>
public sealed class ActivityWrapper : IActivity {
    private readonly Activity _activity;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ActivityWrapper"/> class.
    /// </summary>
    /// <param name="activity">
    ///     The current activity.
    /// </param>
    public ActivityWrapper(Activity activity) {
        _activity = activity;
    }

    ~ActivityWrapper() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public IActivity SetTag(string key, object? value) {
        BlockAccessAfterDispose();
        
        _activity.SetTag(key, value);

        return this;
    }

    /// <inheritdoc />
    public IActivity AddException(Exception exception) {
        BlockAccessAfterDispose();

        _activity.AddException(exception);

        return this;
    }

    /// <inheritdoc />
    public IActivity SetStatus(ActivityStatusCode code, string? description = null) {
        BlockAccessAfterDispose();

        _activity.SetStatus(code, description);
        
        return this;
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _activity.Dispose();
        }

        _disposed = true;
    }
}