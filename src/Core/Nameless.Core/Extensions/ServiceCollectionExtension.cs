using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Infrastructure;

namespace Nameless;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers <see cref="IApplicationContext"/> implementation in the service collection.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="useCommonAppDataFolder">
    /// Whether it will use the common data folder or the user level data folder.
    /// </param>
    /// <param name="appVersion">The application version.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self,
                                                                bool useCommonAppDataFolder = false,
                                                                Version? appVersion = null)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IApplicationContext>(provider => {
                      var hostEnv = provider.GetRequiredService<IHostEnvironment>();
                      var logger = provider.GetLogger<ApplicationContext>();

                      return new ApplicationContext(environment: hostEnv.EnvironmentName,
                                                    appName: hostEnv.ApplicationName,
                                                    useCommonAppDataFolder,
                                                    appVersion ?? new Version(major: 1, minor: 0, build: 0),
                                                    logger);
                  });

    /// <summary>
    /// Registers service <see cref="IClock"/>
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection RegisterClock(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IClock, Clock>();
}