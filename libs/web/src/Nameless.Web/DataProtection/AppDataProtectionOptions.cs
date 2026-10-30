using Microsoft.AspNetCore.DataProtection;
using Nameless.Configuration;

namespace Nameless.Web.DataProtection;

/// <summary>
///     Data protection configuration options.
/// </summary>
[ConfigurationSectionName("DataProtection")]
public sealed class AppDataProtectionOptions : DataProtectionOptions {
    /// <summary>
    ///     Whether it should use the file system.
    /// </summary>
    public bool UseFileSystem { get; set; }

    /// <summary>
    ///     Gets or sets the path in the file system.
    /// </summary>
    public string? FileSystemPath { get; set; }
}
