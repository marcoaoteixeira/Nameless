namespace Nameless.Compression.Responses;

/// <summary>
///     Represents the response metadata of a compressed file archive.
/// </summary>
/// <param name="DestinationFilePath">
///     The full path to the archive file.
/// </param>
public readonly record struct CompressMetadata(string DestinationFilePath) {
    /// <summary>
    ///     Whether the file specified by the current
    ///     file path exists or not.
    /// </summary>
    public bool IsDestinationFileAvailable => File.Exists(DestinationFilePath);
}