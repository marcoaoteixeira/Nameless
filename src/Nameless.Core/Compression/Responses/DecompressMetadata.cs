namespace Nameless.Compression.Responses;

/// <summary>
///     Represents the metadata for a <see cref="DecompressResponse"/>.
/// </summary>
/// <param name="DirectoryPath">
///     The path to the destination directory were the file was decompressed.
/// </param>
public readonly record struct DecompressMetadata(string DirectoryPath) {
    /// <summary>
    ///     Whether the destination directory exists or not.
    /// </summary>
    public bool IsDirectoryAvailable => Directory.Exists(DirectoryPath);
}