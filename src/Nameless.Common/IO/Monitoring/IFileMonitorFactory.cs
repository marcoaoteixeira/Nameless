namespace Nameless.IO.Monitoring;

/// <summary>
///     Creates <see cref="IFileMonitor" /> instances.
/// </summary>
public interface IFileMonitorFactory {
    /// <summary>
    ///     Creates a monitoring that is not started yet.
    /// </summary>
    /// <param name="root">
    ///     The folder to monitor.
    /// </param>
    /// <param name="glob">
    ///     The glob pattern, relative to <paramref name="root" />.
    /// </param>
    /// <returns>
    ///     The new monitoring. The caller owns and must dispose it.
    /// </returns>
    IFileMonitor Create(string root, string glob);
}