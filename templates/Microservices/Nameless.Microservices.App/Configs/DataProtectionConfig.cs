using Microsoft.AspNetCore.DataProtection;
using Nameless.Microservices.App.Infrastructure;

namespace Nameless.Microservices.App.Configs;

public static class DataProtectionConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureDataProtection() {
            var options = self.Configuration
                              .GetSection(nameof(AppDataProtectionOptions))
                              .Get<AppDataProtectionOptions>() ?? new AppDataProtectionOptions();

            var builder = self.Services
                              .AddDataProtection(opts => opts.ApplicationDiscriminator = options.ApplicationDiscriminator)
                              .SetApplicationName(self.Environment.ApplicationName);

            if (options.UseFileSystem && !string.IsNullOrWhiteSpace(options.FileSystemPath)) {
                builder.PersistKeysToFileSystem(new DirectoryInfo(options.FileSystemPath));
            }

            return self;
        }
    }
}
