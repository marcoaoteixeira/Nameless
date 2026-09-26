namespace Nameless.IO.Monitoring;

/// <summary>
///     Outcome of an <see cref="IFileProbe" /> check.
/// </summary>
public enum FileProbeResult {
    /// <summary>
    ///     The path does not exist.
    /// </summary>
    NotFound,

    /// <summary>
    ///     The path is a directory.
    /// </summary>
    Directory,

    /// <summary>
    ///     The file exists but another process holds it.
    /// </summary>
    Locked,

    /// <summary>
    ///     The file exists and was opened with <see cref="FileShare.None" />.
    /// </summary>
    Available
}