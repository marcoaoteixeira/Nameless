namespace Nameless.Compression;

/// <summary>
///     Represents a file entry in the compression request.
/// </summary>
/// <param name="Path">
///     The path to the file.
/// </param>
/// <param name="DirectoryPath">
///     The directory path to be created inside the compressed file.
///     If <see langword="null"/>, the file will appear in the root
///     directory of the zip archive.
/// </param>
public readonly record struct FileEntry(string Path, string? DirectoryPath);