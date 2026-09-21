namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IFileMonitorFactory" /> that builds
///     <see cref="FileMonitor" /> over the real file system.
/// </summary>
public sealed class FileMonitorFactory : IFileMonitorFactory {
    private readonly TimeProvider? _timeProvider;
    private readonly FileMonitorOptions? _options;

    /// <summary>
    ///     Initializes a new instance of <see cref="FileMonitorFactory"/>
    ///     class.
    /// </summary>
    /// <param name="timeProvider">
    ///     The clock for timers; the system clock when
    ///     <see langword="null" />.
    /// </param>
    /// <param name="options">
    ///     The tuning options shared by every monitoring; defaults when
    ///     <see langword="null" />.
    /// </param>
    public FileMonitorFactory(TimeProvider? timeProvider = null, FileMonitorOptions? options = null)
    {
        _timeProvider = timeProvider;
        _options = options;
    }

    /// <inheritdoc />
    public IFileMonitor Create(string root, string glob) {
        return new FileMonitor(
            root,
            glob,
            new FileSystemWatcherAdapter(),
            FileProbe.Instance,
            _timeProvider ?? TimeProvider.System,
            _options
        );
    }
}
