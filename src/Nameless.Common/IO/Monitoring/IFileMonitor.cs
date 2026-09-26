namespace Nameless.IO.Monitoring;

/// <summary>
///     Watches a folder and raises glob-filtered, debounced file events.
/// </summary>
/// <remarks>
///     Only one handler is accepted per event, and handlers must be set
///     before <see cref="Start" />. Handlers run on a thread-pool thread,
///     one at a time and in event order. A slow handler delays later
///     notifications but never the watcher.
/// </remarks>
public interface IFileMonitor : IDisposable {
    /// <summary>
    ///     Gets the folder being monitored.
    /// </summary>
    string Root { get; }

    /// <summary>
    ///     Gets the glob pattern, relative to <see cref="Root" />, that a
    ///     path must match to be reported.
    /// </summary>
    string Glob { get; }

    /// <summary>
    ///     Sets the handler for created files. Raised once the file is
    ///     available for exclusive access.
    /// </summary>
    /// <param name="action">
    ///     The handler.
    /// </param>
    void OnCreated(Action<FileCreatedEvent> action);

    /// <summary>
    ///     Sets the handler for renamed or moved files. Raised immediately.
    /// </summary>
    /// <param name="action">
    ///     The handler.
    /// </param>
    void OnRenamed(Action<FileRenamedEvent> action);

    /// <summary>
    ///     Sets the handler for deleted files.
    /// </summary>
    /// <param name="action">
    ///     The handler.
    /// </param>
    /// <remarks>
    ///     Raised after <see cref="FileMonitorOptions.ReplaceGracePeriod" />,
    ///     unless the path is replaced meanwhile, in which case a single
    ///     <see cref="FileChangedEvent" /> is raised instead.
    /// </remarks>
    void OnDeleted(Action<FileDeletedEvent> action);

    /// <summary>
    ///     Sets the handler for changed files. Raised once, after the changes
    ///     settle and the file is available.
    /// </summary>
    /// <param name="action">
    ///     The handler.
    /// </param>
    void OnChanged(Action<FileChangedEvent> action);

    /// <summary>
    ///     Sets the handler for monitoring errors.
    /// </summary>
    /// <param name="action">
    ///     The handler.
    /// </param>
    void OnError(Action<FileMonitorErrorEvent> action);

    /// <summary>
    ///     Starts raising events. Calling it again has no effect.
    /// </summary>
    void Start();
}
