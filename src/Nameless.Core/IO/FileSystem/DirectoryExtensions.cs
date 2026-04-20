namespace Nameless.IO.FileSystem;

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
        public bool IsEmpty => !self.GetFiles(searchPattern: "*", recursive: true).Any();

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
            return self.GetFiles(searchPattern: "*", recursive: true);
        }

        /// <summary>
        ///     Retrieves all files from the current directory.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search pattern. Default is <c>*</c> char.
        /// </param>
        /// <returns>
        ///     A collection of <see cref="IFile"/> representing all
        ///     files from the directory.
        /// </returns>
        /// <remarks>
        ///     The method only looks in the top directory.
        /// </remarks>
        public IEnumerable<IFile> GetFiles(string searchPattern) {
            return self.GetFiles(searchPattern, recursive: false);
        }

        /// <summary>
        ///     Retrieves all files from the current directory.
        /// </summary>
        /// <param name="recursive">
        ///     Whether it should retrieve from subdirectories.
        ///     Default is <see langword="false"/>.
        /// </param>
        /// <returns>
        ///     A collection of <see cref="IFile"/> representing all
        ///     files from the directory.
        /// </returns>
        /// <remarks>
        ///     The method looks for all kind of files.
        /// </remarks>
        public IEnumerable<IFile> GetFiles(bool recursive) {
            return self.GetFiles(searchPattern: "*", recursive);
        }

        /// <summary>
        ///     Retrieves all directories inside the current directory.
        /// </summary>
        /// <returns>
        ///     A collection of <see cref="IDirectory"/> representing all
        ///     directories inside the current directory.
        /// </returns>
        /// <remarks>
        ///     Search pattern matches all directories and the lookup is
        ///     recursive.
        /// </remarks>
        public IEnumerable<IDirectory> GetDirectories() {
            return self.GetDirectories(searchPattern: "*", recursive: true);
        }

        /// <summary>
        ///     Retrieves all directories inside the current directory.
        /// </summary>
        /// <param name="searchPattern">
        ///     The search pattern. Default is <c>*</c> char.
        /// </param>
        /// <returns>
        ///     A collection of <see cref="IDirectory"/> representing all
        ///     directories inside the current directory.
        /// </returns>
        /// <remarks>
        ///     The method only looks in the top directory.
        /// </remarks>
        public IEnumerable<IDirectory> GetDirectories(string searchPattern) {
            return self.GetDirectories(searchPattern, recursive: false);
        }

        /// <summary>
        ///     Retrieves all directories inside the current directory.
        /// </summary>
        /// <param name="recursive">
        ///     Whether it should retrieve all subdirectories.
        /// </param>
        /// <returns>
        ///     A collection of <see cref="IDirectory"/> representing all
        ///     directories inside the current directory.
        /// </returns>
        /// <remarks>
        ///     The search filter will match any directory.
        /// </remarks>
        public IEnumerable<IDirectory> GetDirectories(bool recursive) {
            return self.GetDirectories(searchPattern: "*", recursive);
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