using Microsoft.Extensions.Configuration;

namespace Nameless.Testing.Tools.Helpers;

public static class ConfigurationHelper {
    private static readonly Lazy<IConfiguration> Configuration = new(CreateConfiguration);

    public static IConfiguration Instance => Configuration.Value;

    private static IConfiguration CreateConfiguration() {
        return new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddEnvironmentVariables()
               .AddJsonFile("AppSettings.json", optional: true)
               .Build();
    }
}