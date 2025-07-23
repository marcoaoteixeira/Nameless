using Microsoft.AspNetCore.DataProtection;

namespace Nameless.Microservices.App.Infrastructure;

public class AppDataProtectionOptions : DataProtectionOptions {
    public bool UseFileSystem { get; set; }
    public string? FileSystemPath { get; set; }
}
