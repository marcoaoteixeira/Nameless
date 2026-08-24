namespace Nameless.IO.Monitoring;

/// <summary>
///     Exception thrown by <see cref="FileMonitor"/> when the
///     underlying file provider does not support active change callbacks.
/// </summary>
public sealed class FileMonitorException : Exception {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="FileMonitorException"/>.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    public FileMonitorException(string message) : base(message) { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="FileMonitorException"/> with an inner
    ///     exception.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="inner">
    ///     The inner exception.
    /// </param>
    public FileMonitorException(string message, Exception inner) : base(message, inner) { }
}
