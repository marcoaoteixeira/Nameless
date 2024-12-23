namespace Nameless.FileSystem;

/// <summary>
/// Directory contract.
/// </summary>
public interface IDirectory {
    /// <summary>
    /// Gets the path to the directory.
    /// </summary>
    string Path { get; }
}