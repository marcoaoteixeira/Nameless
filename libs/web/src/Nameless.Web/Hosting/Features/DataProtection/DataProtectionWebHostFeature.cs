using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Configuration;
using Nameless.Hosting;
using Nameless.Web.DataProtection;

namespace Nameless.Web.Hosting.Features.DataProtection;

/// <summary>
///     Registers data protection, configured through
///     <see cref="AppDataProtectionOptions"/>.
/// </summary>
public class DataProtectionWebHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = builder.Configuration
                             .GetSectionFor<AppDataProtectionOptions>()
                             .GetOrCreate<AppDataProtectionOptions>();

        var applicationName = builder.Environment.ApplicationName;
        var dataProtection = builder.Services.AddDataProtection(
            opts => opts.ApplicationDiscriminator = !string.IsNullOrWhiteSpace(options.ApplicationDiscriminator)
                ? options.ApplicationDiscriminator
                : applicationName
        );

        if (options.UseFileSystem && !string.IsNullOrWhiteSpace(options.FileSystemPath)) {
            dataProtection.PersistKeysToFileSystem(new DirectoryInfo(options.FileSystemPath));
        }
    }
}
