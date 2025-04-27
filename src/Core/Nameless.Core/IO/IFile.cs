namespace Nameless.IO;

/// <summary>
/// Defines properties to represent a file.
/// </summary>
public interface IFile {
    /// <summary>
    /// Gets the path to the file.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether the file exists or not.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets the file creation date.
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the file last write time.
    /// </summary>
    DateTime LastWriteAt { get; }
}