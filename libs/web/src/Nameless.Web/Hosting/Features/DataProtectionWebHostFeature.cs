using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Configuration;
using Nameless.Hosting;
using Nameless.Web.DataProtection;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers data protection, configured through
///     <see cref="AppDataProtectionOptions"/>.
/// </summary>
public class DataProtectionWebHostFeature : HostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = builder.Configuration
                             .GetSectionFor<AppDataProtectionOptions>()
                             .GetOrThrow<AppDataProtectionOptions>();

        var dataProtection = builder.Services
                                    .AddDataProtection(opts => opts.ApplicationDiscriminator = options.ApplicationDiscriminator)
                                    .SetApplicationName(builder.Environment.ApplicationName);

        if (options.UseFileSystem && !string.IsNullOrWhiteSpace(options.FileSystemPath)) {
            dataProtection.PersistKeysToFileSystem(new DirectoryInfo(options.FileSystemPath));
        }
    }
}
