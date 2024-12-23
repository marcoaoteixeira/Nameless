namespace Nameless.FileSystem;

/// <summary>
/// Directory service contract.
/// </summary>
public interface IDirectoryService {
    /// <summary>
    /// Retrieves all files from a directory.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="recursive">Whether it should look into children directories.</param>
    /// <returns>
    /// An array of <see cref="IFile"/> containing a representation of all files from the directory.
    /// </returns>
    IFile[] GetFiles(string directoryPath, string filter, bool recursive);

    /// <summary>
    /// Retrieves all children directories.
    /// </summary>
    /// <param name="rootPath">The root directory path.</param>
    /// <param name="filter">The filter.</param>
    /// <param name="recursive">Whether it should look for children directories.</param>
    /// <returns>
    /// An array of <see cref="IDirectory"/> containing a representation of all directories inside the root directory.
    /// </returns>
    IDirectory[] GetDirectories(string rootPath, string filter, bool recursive);

    /// <summary>
    /// Creates a new directory.
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    IDirectory Create(string directoryPath);

    /// <summary>
    /// Checks if a directory exists.
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <returns>
    /// <c>true</c> if it exists; otherwise <c>false</c>.
    /// </returns>
    bool Exists(string directoryPath);

    /// <summary>
    /// Deletes a directory.
    /// </summary>
    /// <param name="directoryPath">The directory path</param>
    /// <param name="recursive">Whether it should delete all children content.</param>
    void Delete(string directoryPath, bool recursive);
}

public static class DirectoryOperatorExtension {
    private const string INCLUDE_ALL = "*.*";

    public static IFile[] GetFiles(this IDirectoryService self, string directoryPath, string? filter = null, bool recursive = false)
        => self.GetFiles(directoryPath, filter ?? INCLUDE_ALL, recursive);

    public static void Delete(this IDirectoryService self, string directoryPath, bool recursive = false)
        => self.Delete(directoryPath, recursive);

    public static IDirectory[] GetDirectories(this IDirectoryService self, string rootPath, string? filter = null, bool recursive = false)
        => self.GetDirectories(rootPath, filter ?? INCLUDE_ALL, recursive);
}