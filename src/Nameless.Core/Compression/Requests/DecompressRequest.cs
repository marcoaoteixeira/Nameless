namespace Nameless.Compression.Requests;

/// <summary>
///     Represents a decompress request.
/// </summary>
public class DecompressRequest {
    /// <summary>
    ///     Gets or sets the source file path.
    /// </summary>
    public required string SourceFilePath { get; set; }

    /// <summary>
    ///     Gets or sets the destination path.
    ///     If <see cref="DestinationDirectoryPath"/> is
    ///     <see langword="null"/>, empty or only whitespace, defaults to the
    ///     system's temporary directory combined with the source file name.
    /// </summary>
    /// <remarks>
    ///     When a custom path is not provided, the final path is constructed
    ///     as: <c>Path.Combine(Path.GetTempPath(), SourceFileName)</c>.
    /// </remarks>
    public string? DestinationDirectoryPath { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="DecompressRequest"/>
    ///     class.
    /// </summary>
    /// <param name="sourceFilePath">
    ///     The source file path.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="sourceFilePath"/> is
    ///     <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="sourceFilePath"/> is
    ///     empty or only whitespace.
    /// </exception>
    public DecompressRequest(string sourceFilePath) {
        SourceFilePath = Throws.When.NullOrWhiteSpace(sourceFilePath);
    }
}