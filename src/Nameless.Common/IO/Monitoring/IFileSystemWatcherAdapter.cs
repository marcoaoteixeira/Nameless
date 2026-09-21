namespace Nameless.IO.Monitoring;

/// <summary>
///     Abstraction over <see cref="FileSystemWatcher" /> so monitoring can be
///     tested without touching the disk.
/// </summary>
public interface IFileSystemWatcherAdapter : IDisposable {
    /// <summary>
    ///     Gets or sets the folder to watch.
    /// </summary>
    string Path { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether subfolders are watched too.
    /// </summary>
    bool IncludeSubdirectories { get; set; }

    /// <summary>
    ///     Gets or sets the size, in bytes, of the internal buffer.
    /// </summary>
    int InternalBufferSize { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether events are being raised.
    /// </summary>
    bool EnableRaisingEvents { get; set; }

    /// <summary>
    ///     Occurs when a file or folder is created.
    /// </summary>
    event FileSystemEventHandler Created;

    /// <summary>
    ///     Occurs when a file or folder changes.
    /// </summary>
    event FileSystemEventHandler Changed;

    /// <summary>
    ///     Occurs when a file or folder is deleted.
    /// </summary>
    event FileSystemEventHandler Deleted;

    /// <summary>
    ///     Occurs when a file or folder is renamed.
    /// </summary>
    event RenamedEventHandler Renamed;

    /// <summary>
    ///     Occurs when the watcher fails, for example on buffer overflow.
    /// </summary>
    event ErrorEventHandler Error;
}
