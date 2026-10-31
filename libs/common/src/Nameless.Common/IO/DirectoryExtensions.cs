namespace Nameless.IO;

/// <summary>
///     <see cref="IDirectory"/> extension methods.
/// </summary>
public static class DirectoryExtensions {
    /// <param name="self">
    ///     The current <see cref="IDirectory"/>.
    /// </param>
    extension(IDirectory self) {
        /// <summary>
        ///     Whether the directory does not contain any files.
        /// </summary>
        public bool IsEmpty => !self.GetFiles(glob: "*").Any();

        /// <summary>
        ///     Retrieves all files from the current directory.
        /// </summary>
        /// <returns>
        ///     A collection of <see cref="IFile"/> representing all
        ///     files from the directory.
        /// </returns>
        /// <remarks>
        ///     Search pattern matches all files and the lookup is
        ///     recursive.
        /// </remarks>
        public IEnumerable<IFile> GetFiles() {
            return self.GetFiles(glob: "**/*");
        }

        /// <summary>
        ///     Deletes the directory.
        /// </summary>
        /// <param name="recursive">
        ///     Whether it should delete subdirectories.
        ///     Default is <see langword="false"/>.
        /// </param>
        public void Delete(bool recursive = false) {
            self.Delete(recursive);
        }
    }
}