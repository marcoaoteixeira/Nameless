namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IFileSystemWatcherAdapter" /> backed by a
///     <see cref="FileSystemWatcher" />.
/// </summary>
/// <remarks>
///     Only file name, last write and size notifications are requested,
///     so folder events are not raised.
/// </remarks>
public sealed class FileSystemWatcherAdapter : IFileSystemWatcherAdapter {
    private readonly FileSystemWatcher _watcher = new() {
        NotifyFilter = NotifyFilters.FileName |
                       NotifyFilters.LastWrite |
                       NotifyFilters.Size
    };

    /// <inheritdoc />
    public string Path {
        get => _watcher.Path;
        set => _watcher.Path = value;
    }

    /// <inheritdoc />
    public bool IncludeSubdirectories {
        get => _watcher.IncludeSubdirectories;
        set => _watcher.IncludeSubdirectories = value;
    }

    /// <inheritdoc />
    public int InternalBufferSize {
        get => _watcher.InternalBufferSize;
        set => _watcher.InternalBufferSize = value;
    }

    /// <inheritdoc />
    public bool EnableRaisingEvents {
        get => _watcher.EnableRaisingEvents;
        set => _watcher.EnableRaisingEvents = value;
    }

    /// <inheritdoc />
    public event FileSystemEventHandler Created {
        add => _watcher.Created += value;
        remove => _watcher.Created -= value;
    }

    /// <inheritdoc />
    public event FileSystemEventHandler Changed {
        add => _watcher.Changed += value;
        remove => _watcher.Changed -= value;
    }

    /// <inheritdoc />
    public event FileSystemEventHandler Deleted {
        add => _watcher.Deleted += value;
        remove => _watcher.Deleted -= value;
    }

    /// <inheritdoc />
    public event RenamedEventHandler Renamed {
        add => _watcher.Renamed += value;
        remove => _watcher.Renamed -= value;
    }

    /// <inheritdoc />
    public event ErrorEventHandler Error {
        add => _watcher.Error += value;
        remove => _watcher.Error -= value;
    }

    /// <inheritdoc />
    public void Dispose() {
        _watcher.Dispose();
    }
}
