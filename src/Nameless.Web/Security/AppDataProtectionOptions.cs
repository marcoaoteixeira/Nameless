using Microsoft.AspNetCore.DataProtection;
using Nameless.Attributes;

namespace Nameless.Web.Security;

[ConfigurationSectionName("DataProtection")]
public sealed class AppDataProtectionOptions : DataProtectionOptions {
    public bool UseFileSystem { get; set; }

    public string? FileSystemPath { get; set; }
}
