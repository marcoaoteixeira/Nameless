namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="IFile"/> extension methods.
/// </summary>
public static class FileExtensions {
    /// <summary>
    ///     Opens the file with the specified mode, access, and
    ///     share options.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IFile"/>.
    /// </param>
    /// <param name="mode">
    ///     The file mode to use when opening the file.
    /// </param>
    /// <param name="access">
    ///     The file access level to use when opening the file.
    /// </param>
    /// <param name="share">
    ///     The file share mode to use when opening the file.
    /// </param>
    /// <returns>
    ///     A <see cref="Stream"/> representing the opened file.
    /// </returns>
    public static Stream Open(this IFile self, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.ReadWrite) {
        return self.Open(mode, access, share);
    }

    /// <summary>
    ///     Copies the file to the specified destination path.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IFile"/>.
    /// </param>
    /// <param name="destinationRelativePath">
    ///     The relative path where the file should be copied.
    /// </param>
    /// <param name="overwrite">
    ///     Whether to overwrite the file if it already exists
    ///     at the destination.
    /// </param>
    /// <returns>
    ///     The copied <see cref="IFile"/>.
    /// </returns>
    public static IFile Copy(this IFile self, string destinationRelativePath, bool overwrite = false) {
        return self.Copy(destinationRelativePath, overwrite);
    }
}
