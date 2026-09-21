using Nameless.Configuration;
using Nameless.IO.Monitoring;

namespace Nameless.IO.System;

/// <summary>
///     The options for configuring the <see cref="IFileProvider"/>.
/// </summary>
[ConfigurationSectionName("FileProvider")]
public class FileProviderOptions {
    /// <summary>
    ///     Gets or sets the root directory for file system operations.
    /// </summary>
    public string Root { get; set; } = AppContext.BaseDirectory;

    /// <summary>
    ///     Whether to allow operations outside the root directory.
    /// </summary>
    public bool AllowOperationOutsideRoot { get; set; }

    /// <summary>
    ///     Gets or sets the file monitor options.
    /// </summary>
    public FileMonitorOptions FileMonitorOptions { get; set; } = new();

    internal FileProviderOptions Validate() {
        Throws.When.NullOrWhiteSpace(Root, message: "Root directory not provided.");
        Throws.When.DirectoryNotFound(Root, message: "Root directory was not found.");

        return this;
    }
}