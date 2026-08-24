namespace Nameless.IO.Monitoring;

/// <summary>
///     Provides meanings of create <see cref="IFileMonitor"/> instances.
/// </summary>
public interface IFileMonitorProvider {
    /// <summary>
    ///     Creates a new <see cref="IFileMonitor"/> instance that monitor
    ///     the provided root path.
    /// </summary>
    /// <param name="options">
    ///     The File System Monitor options.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IFileMonitor"/>.
    /// </returns>
    IFileMonitor Create(Action<FileMonitorOptions> options);
}