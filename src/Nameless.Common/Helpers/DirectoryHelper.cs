namespace Nameless.Helpers;

/// <summary>
///     Directory helper.
/// </summary>
public static class DirectoryHelper {
    /// <summary>
    ///     Copies the content of the source directory to destination directory.
    /// </summary>
    /// <param name="sourceDirectory">
    ///     The source directory.
    /// </param>
    /// <param name="destinationDirectory">
    ///     The destination directory.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <exception cref="DirectoryNotFoundException">
    ///     When the source directory is not found.
    /// </exception>
    public static void CopyDirectory(string sourceDirectory, string destinationDirectory, CancellationToken cancellationToken = default) {
        if (!Directory.Exists(sourceDirectory)) {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
        }

        // ensure directory existence
        Directory.CreateDirectory(destinationDirectory);

        // copy files
        foreach (var file in Directory.EnumerateFiles(sourceDirectory)) {
            cancellationToken.ThrowIfCancellationRequested();

            File.Copy(
                file,
                Path.Combine(destinationDirectory, Path.GetFileName(file)),
                overwrite: true
            );
        }

        // recursively copy directories
        foreach (var directory in Directory.EnumerateDirectories(sourceDirectory)) {
            CopyDirectory(
                sourceDirectory: directory,
                destinationDirectory: Path.Combine(destinationDirectory, Path.GetFileName(directory)),
                cancellationToken
            );
        }
    }
}
