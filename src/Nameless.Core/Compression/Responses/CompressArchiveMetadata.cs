namespace Nameless.Compression.Responses;

/// <summary>
///     Represents the response metadata of a compressed file archive.
/// </summary>
/// <param name="FilePath">
///     The full path to the archive file.
/// </param>
public readonly record struct CompressArchiveMetadata(string FilePath) {
    /// <summary>
    ///     Whether the file specified by the current
    ///     file path exists.
    /// </summary>
    public bool IsFileAvailable => File.Exists(FilePath);
}