using Microsoft.AspNetCore.DataProtection;
using Nameless.Microservices.App.Infrastructure;

namespace Nameless.Microservices.App.Configs;

public static class DataProtectionConfig {
    public static WebApplicationBuilder ConfigureDataProtection(this WebApplicationBuilder self) {
        var dataProtectionOptions = self.Configuration
                                        .GetSection(nameof(AppDataProtectionOptions))
                                        .Get<AppDataProtectionOptions>() ?? new AppDataProtectionOptions();

        var dataProtectionBuilder = self.Services
                                        .AddDataProtection(opts => opts.ApplicationDiscriminator = dataProtectionOptions.ApplicationDiscriminator)
                                        .SetApplicationName(self.Environment.ApplicationName);

        if (dataProtectionOptions.UseFileSystem && !string.IsNullOrWhiteSpace(dataProtectionOptions.FileSystemPath)) {
            dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionOptions.FileSystemPath));
        }

        return self;
    }
}
