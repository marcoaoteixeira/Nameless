using Nameless.Attributes;
using Nameless.Helpers;

namespace Nameless.IO.FileSystem;

/// <summary>
///     The options for configuring the <see cref="IFileSystemProvider"/>.
/// </summary>
[ConfigurationSectionName("FileSystemProvider")]
public record FileSystemProviderOptions {
    /// <summary>
    ///     Gets or sets the root directory for file system operations.
    /// </summary>
    public required string Root { get; init; }

    /// <summary>
    ///     Whether to allow operations outside the root directory.
    /// </summary>
    public bool AllowOperationOutsideRoot { get; init; }
}