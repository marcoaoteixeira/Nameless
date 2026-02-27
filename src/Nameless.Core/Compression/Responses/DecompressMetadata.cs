namespace Nameless.Compression.Responses;

/// <summary>
///     Represents the metadata for a <see cref="DecompressResponse"/>.
/// </summary>
/// <param name="DestinationDirectoryPath">
///     The destination directory path.
/// </param>
public readonly record struct DecompressMetadata(string DestinationDirectoryPath) {
    /// <summary>
    ///     Whether the destination directory exists or not.
    /// </summary>
    public bool IsDestinationDirectoryAvailable => Directory.Exists(DestinationDirectoryPath);
}