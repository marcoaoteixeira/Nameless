using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Infrastructure;

namespace Nameless;

public static class ServiceCollectionExtensions {
    private static readonly Version DefaultAppVersion = new(1, 0, 0);

    /// <summary>
    ///     Registers <see cref="IApplicationContext" /> implementation in the service collection.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    /// <param name="useCommonAppDataFolder">
    ///     Whether it will use the common data folder or the user level data folder.
    /// </param>
    /// <param name="appVersion">The application version.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" />, so other actions can be chained.
    /// </returns>
    public static IServiceCollection ConfigureApplicationContext(this IServiceCollection self, bool useCommonAppDataFolder = false, Version? appVersion = null) {
        return self.AddSingleton<IApplicationContext>(provider => {
            var hostEnv = provider.GetRequiredService<IHostEnvironment>();
            var logger = provider.GetLogger<ApplicationContext>();

            return new ApplicationContext(hostEnv.EnvironmentName,
                hostEnv.ApplicationName,
                useCommonAppDataFolder,
                appVersion ?? DefaultAppVersion,
                logger);
        });
    }
}