namespace Nameless.IO.FileSystem;

/// <summary>
///     <see cref="IFile"/> extension methods.
/// </summary>
public static class FileExtensions {
    /// <param name="self">
    ///     The current <see cref="IFile"/>.
    /// </param>
    extension(IFile self) {
        /// <summary>
        ///     Opens the file with the specified mode, access, and
        ///     share options.
        /// </summary>
        /// <param name="mode">
        ///     The file mode to use when opening the file.
        ///     Default is <see cref="FileMode.OpenOrCreate"/>.
        /// </param>
        /// <param name="access">
        ///     The file access level to use when opening the file.
        ///     Default is <see cref="FileAccess.ReadWrite"/>.
        /// </param>
        /// <param name="share">
        ///     The file share mode to use when opening the file.
        ///     Default is <see cref="FileShare.ReadWrite"/>.
        /// </param>
        /// <returns>
        ///     A <see cref="Stream"/> representing the opened file.
        /// </returns>
        public Stream Open(FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.ReadWrite,
            FileShare share = FileShare.ReadWrite) {
            return self.Open(mode, access, share);
        }

        /// <summary>
        ///     Copies the file to the specified destination path.
        /// </summary>
        /// <param name="destinationRelativePath">
        ///     The relative path where the file should be copied.
        /// </param>
        /// <param name="overwrite">
        ///     Whether to overwrite the file if it already exists
        ///     at the destination.
        ///     Default is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     The copied <see cref="IFile"/>.
        /// </returns>
        public IFile Copy(string destinationRelativePath, bool overwrite = false) {
            return self.Copy(destinationRelativePath, overwrite);
        }
    }
}