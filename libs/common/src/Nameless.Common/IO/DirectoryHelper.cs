namespace Nameless.IO;

/// <summary>
///     SystemDirectory helper.
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
        if (!SysDirectory.Exists(sourceDirectory)) {
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
        }

        // ensure directory existence
        SysDirectory.CreateDirectory(destinationDirectory);

        // copy files
        foreach (var file in SysDirectory.EnumerateFiles(sourceDirectory)) {
            cancellationToken.ThrowIfCancellationRequested();

            SysFile.Copy(
                file,
                SysPath.Combine(destinationDirectory, SysPath.GetFileName(file)),
                overwrite: true
            );
        }

        // recursively copy directories
        foreach (var directory in SysDirectory.EnumerateDirectories(sourceDirectory)) {
            CopyDirectory(
                sourceDirectory: directory,
                destinationDirectory: SysPath.Combine(destinationDirectory, SysPath.GetFileName(directory)),
                cancellationToken
            );
        }
    }
}
