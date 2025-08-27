namespace Nameless.IO.FileSystem;

public interface IFileSystem {
    /// <summary>
    ///     Gets the root directory for file system operations.
    /// </summary>
    string Root { get; }

    /// <summary>
    ///     Retrieves a directory by its relative path.
    /// </summary>
    /// <param name="relativePath">
    ///     The relative path of the directory to retrieve.
    /// </param>
    /// <returns>
    ///     A <see cref="IDirectory"/> representing the specified directory.
    /// </returns>
    IDirectory GetDirectory(string relativePath);

    /// <summary>
    ///     Retrieves a file by its relative path.
    /// </summary>
    /// <param name="relativePath">
    ///     The relative path of the file to retrieve.
    /// </param>
    /// <returns>
    ///     A <see cref="IFile"/> representing the specified file.
    /// </returns>
    IFile GetFile(string relativePath);

    /// <summary>
    ///     Retrieves the full path for a given relative path.
    /// </summary>
    /// <param name="relativePath">
    ///     The relative path to convert to a full path.
    /// </param>
    /// <returns>
    ///     Returns the full path as a string.
    /// </returns>
    string GetFullPath(string relativePath);
}