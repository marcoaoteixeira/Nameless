namespace Nameless.FileSystem;

/// <summary>
/// Path service contract.
/// </summary>
public interface IPathService {
    /// <summary>
    /// Returns a uniquely named temporary file on disk.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> with the path to the temporary file.
    /// </returns>
    string GetTempFileName();

    /// <summary>
    /// Gets the file extension including the period "."
    /// </summary>
    /// <param name="filePath">The path.</param>
    /// <returns>
    /// A <see cref="string"/> containing the file extension.
    /// </returns>
    string? GetExtension(string filePath);

    /// <summary>
    /// Gets the file name.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>
    /// A <see cref="string"/> containing the file name.
    /// </returns>
    string? GetFileName(string filePath);

    /// <summary>
    /// Gets the file name without the extension.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>
    /// A <see cref="string"/> containing the file name without the extension.
    /// </returns>
    string? GetFileNameWithoutExtension(string filePath);

    /// <summary>
    /// Combines an array of string into a path.
    /// </summary>
    /// <param name="parts">The path parts.</param>
    /// <returns>A path.</returns>
    string Combine(params string[] parts);
}