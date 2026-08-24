namespace Nameless.IO.Monitoring;

/// <summary>
///     Represents a smart file system watcher that monitors a directory for
///     changes and fires typed callbacks only when file operations are fully
///     complete.
/// </summary>
public interface IFileMonitor : IAsyncDisposable {
    /// <summary>
    ///     Registers the callback to invoke when a file is created.
    ///     Calling this method again overwrites the previous callback.
    /// </summary>
    /// <param name="callback">
    ///     The delegate to invoke.
    /// </param>
    void OnCreated(FileMonitorDelegate<FileMonitorEventArgs> callback);

    /// <summary>
    ///     Registers the callback to invoke when a file is deleted.
    ///     Calling this method again overwrites the previous callback.
    /// </summary>
    /// <param name="callback">
    ///     The delegate to invoke.
    /// </param>
    void OnDeleted(FileMonitorDelegate<FileMonitorEventArgs> callback);

    /// <summary>
    ///     Registers the callback to invoke when a file is renamed.
    ///     Calling this method again overwrites the previous callback.
    /// </summary>
    /// <param name="callback">
    ///     The delegate to invoke.
    /// </param>
    void OnRenamed(FileMonitorDelegate<FileMonitorEventArgs> callback);

    /// <summary>
    ///     Registers the callback to invoke when a file is changed.
    ///     Calling this method again overwrites the previous callback.
    /// </summary>
    /// <param name="callback">
    ///     The delegate to invoke.
    /// </param>
    void OnChanged(FileMonitorDelegate<FileMonitorEventArgs> callback);

    /// <summary>
    ///     Registers the callback to invoke when a watcher error occurs.
    ///     Calling this method again overwrites the previous callback.
    /// </summary>
    /// <param name="callback">
    ///     The delegate to invoke.
    /// </param>
    void OnError(FileMonitorDelegate<FileMonitorErrorEventArgs> callback);

    /// <summary>
    ///     Starts watching the configured directory.
    /// </summary>
    /// <param name="cancellationToken">
    ///     Propagates notification that the operation should be cancelled.
    /// </param>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Stops watching and waits for any in-flight probes to complete.
    /// </summary>
    /// <param name="cancellationToken">
    ///     Propagates notification that the operation should be cancelled.
    /// </param>
    Task StopAsync(CancellationToken cancellationToken = default);
}
