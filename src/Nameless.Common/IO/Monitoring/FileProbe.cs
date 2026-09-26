namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IFileProbe" /> that opens the file with
///     <see cref="FileShare.None" />.
/// </summary>
public sealed class FileProbe : IFileProbe {
    /// <summary>
    ///     Gets the single instance to the file 
    /// </summary>
    public static IFileProbe Instance { get; } = new FileProbe();

    static FileProbe() { }

    private FileProbe() { }

    /// <inheritdoc />
    /// <exception cref="PathTooLongException">
    ///     <paramref name="path" /> exceeds the system limit.
    /// </exception>
    public FileProbeResult Probe(string path) {
        if (SysDirectory.Exists(path)) {
            return FileProbeResult.Directory;
        }

        try {
            using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.None
            );

            return FileProbeResult.Available;
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException) {
            return FileProbeResult.NotFound;
        }
        catch (Exception ex) when (ex is (UnauthorizedAccessException or IOException) and not PathTooLongException) {
            return FileProbeResult.Locked;
        }
    }
}
