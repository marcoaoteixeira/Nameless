namespace Nameless.IO.Monitoring;

/// <summary>
///     A file stayed locked past <see cref="FileMonitorOptions.LockedTooLongAfter" />.
///     The monitoring keeps watching it; this is a heads-up, not a failure.
/// </summary>
public sealed class FileLockedTooLongException : IOException {
    /// <summary>
    ///     Gets the full path of the locked file.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    ///     Gets how long the file has been locked so far.
    /// </summary>
    public TimeSpan LockedFor { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FileLockedTooLongException" /> class.
    /// </summary>
    /// <param name="filePath">
    ///     The full path of the locked file.
    /// </param>
    /// <param name="lockedFor">
    ///     How long the file has been locked so far.
    /// </param>
    public FileLockedTooLongException(string filePath, TimeSpan lockedFor)
        : base($"File '{filePath}' has been locked for {lockedFor}; still watching it.") {
        FilePath = filePath;
        LockedFor = lockedFor;
    }
}
