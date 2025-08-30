namespace Nameless.IO.FileSystem;

/// <summary>
///     The options for configuring the <see cref="IFileSystem"/>.
/// </summary>
public sealed class FileSystemOptions {
    /// <summary>
    ///     Gets or sets the root directory for file system operations.
    /// </summary>
    public string Root { get; set; } = string.Empty;

    /// <summary>
    ///     Whether to allow operations outside the root directory.
    /// </summary>
    public bool AllowOperationOutsideRoot { get; set; }
}