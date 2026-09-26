namespace Nameless.IO.Monitoring;

/// <summary>
///     Inspects the current state of a path on disk.
/// </summary>
public interface IFileProbe {
    /// <summary>
    ///     Checks whether the path is a file that can be opened for exclusive
    ///     access.
    /// </summary>
    /// <param name="path">
    ///     The full path to check.
    /// </param>
    /// <returns>
    ///     The probe outcome.
    /// </returns>
    FileProbeResult Probe(string path);
}