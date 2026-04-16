using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Security;

namespace Nameless.Web.Hosting.Configs;

public static class DataProtectionConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureDataProtection(WebHostSettings settings) {
            if (settings.DisableDataProtection) { return self; }

            var options = self.Configuration.GetOptions<AppDataProtectionOptions>();

            var builder = self.Services
                              .AddDataProtection(opts => opts = options)
                              .SetApplicationName(self.Environment.ApplicationName);

            if (options.UseFileSystem && !string.IsNullOrWhiteSpace(options.FileSystemPath)) {
                builder.PersistKeysToFileSystem(new DirectoryInfo(options.FileSystemPath));
            }

            return self;
        }
    }
}
