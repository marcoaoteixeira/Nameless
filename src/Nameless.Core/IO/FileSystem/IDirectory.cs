namespace Nameless.IO.FileSystem;

/// <summary>
///     Defines a directory in the file system.
/// </summary>
public interface IDirectory {
    /// <summary>
    ///     Gets the name of the directory.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the full path of the directory.
    /// </summary>
    string Path { get; }

    /// <summary>
    ///     Whether the directory exists.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    ///     Creates the directory if it does not exist.
    /// </summary>
    /// <returns>
    ///     The full path of the created directory.
    /// </returns>
    string Create();

    /// <summary>
    ///     Retrieves files in the directory matching the
    ///     specified search pattern.
    /// </summary>
    /// <param name="searchPattern">
    ///     The search pattern to match files.
    /// </param>
    /// <param name="recursive">
    ///     Whether to search recursively in subdirectories.
    /// </param>
    /// <returns>
    ///     A collection of <see cref="IFile"/> objects
    /// </returns>
    IEnumerable<IFile> GetFiles(string searchPattern, bool recursive);

    /// <summary>
    ///     Deletes the directory.
    /// </summary>
    /// <param name="recursive">
    ///     Whether to delete subdirectories and files
    /// </param>
    void Delete(bool recursive);
}