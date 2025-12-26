namespace Nameless.IO.FileSystem;

public static class DirectoryExtensions {
    /// <param name="self">
    ///     The current <see cref="IDirectory"/>.
    /// </param>
    extension(IDirectory self) {
        /// <summary>
        ///     Retrieves all files from the current directory.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search pattern. Default is <c>*</c> char.
        /// </param>
        /// <param name="recursive">
        ///     Whether it should retrieve from subdirectories.
        ///     Default is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     A collection of <see cref="IFile"/> representing all
        ///     files from the directory.
        /// </returns>
        public IEnumerable<IFile> GetFiles(string searchPattern = "*", bool recursive = false) {
            return self.GetFiles(searchPattern, recursive);
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