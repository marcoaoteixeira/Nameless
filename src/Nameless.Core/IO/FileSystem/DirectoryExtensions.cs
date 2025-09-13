namespace Nameless.IO.FileSystem;

public static class DirectoryExtensions {
    /// <summary>
    ///     Retrieves all files from the current directory.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IDirectory"/>.
    /// </param>
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
    public static IEnumerable<IFile> GetFiles(this IDirectory self, string searchPattern = "*", bool recursive = false) {
        return self.GetFiles(searchPattern, recursive);
    }

    /// <summary>
    ///     Deletes the directory.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IDirectory"/>.
    /// </param>
    /// <param name="recursive">
    ///     Whether it should delete subdirectories.
    ///     Default is <see langword="false"/>.
    /// </param>
    public static void Delete(this IDirectory self, bool recursive = false) {
        self.Delete(recursive);
    }
}
