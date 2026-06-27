namespace Nameless.IO.Monitoring;

/// <summary>
///     Exception thrown by <see cref="SmartFileSystemWatcher"/> when the
///     underlying file provider does not support active change callbacks.
/// </summary>
public sealed class SmartFileSystemWatcherException : Exception {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="SmartFileSystemWatcherException"/>.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    public SmartFileSystemWatcherException(string message) : base(message) { }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="SmartFileSystemWatcherException"/> with an inner
    ///     exception.
    /// </summary>
    /// <param name="message">
    ///     The error message.
    /// </param>
    /// <param name="inner">
    ///     The inner exception.
    /// </param>
    public SmartFileSystemWatcherException(string message, Exception inner) : base(message, inner) { }
}
