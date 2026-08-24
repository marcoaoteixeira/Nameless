using Nameless.Attributes;

namespace Nameless.IO.Explorer;

/// <summary>
///     The options for configuring the <see cref="IFileExplorer"/>.
/// </summary>
[ConfigurationSectionName("FileExplorer")]
public class FileExplorerOptions {
    /// <summary>
    ///     Gets or sets the root directory for file system operations.
    /// </summary>
    public required string Root { get; set; }

    /// <summary>
    ///     Whether to allow operations outside the root directory.
    /// </summary>
    public bool AllowOperationOutsideRoot { get; set; }

    internal FileExplorerOptions Validate() {
        Throws.When.NullOrWhiteSpace(Root, message: "Root directory not provided.");

        return Directory.Exists(Root)
            ? this
            : throw new DirectoryNotFoundException("Root directory was not found.");
    }
}