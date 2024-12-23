namespace Nameless.FileSystem;

/// <summary>
/// File system contract.
/// </summary>
public interface IFileSystem {
    /// <summary>
    /// Gets the directory services.
    /// </summary>
    IDirectoryService Directory { get; }

    /// <summary>
    /// Gets the file services.
    /// </summary>
    IFileService File { get; }

    /// <summary>
    /// Gets the path services.
    /// </summary>
    IPathService Path { get; }
}