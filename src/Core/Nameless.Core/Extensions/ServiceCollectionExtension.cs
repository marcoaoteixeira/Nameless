using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.Services;

namespace Nameless;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers <see cref="IApplicationContext"/> implementation in the service collection.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="useAppDataSpecialFolder">
    /// <c>true</c> if it should use environment special folder;
    /// <c>false</c> if it should use <see cref="AppDomain.BaseDirectory"/> + "App_Data"
    /// </param>
    /// <param name="appVersion">The application version.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddApplicationContext(this IServiceCollection self, bool useAppDataSpecialFolder = false, Version? appVersion = null)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IApplicationContext>(provider => new ApplicationContext(hostEnvironment: provider.GetRequiredService<IHostEnvironment>(),
                                                                                        logger: provider.GetRequiredService<ILogger<ApplicationContext>>(),
                                                                                        useSpecialFolder: useAppDataSpecialFolder,
                                                                                        appVersion: appVersion));

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
    public static IServiceCollection AddSystemClock(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IClock, SystemClock>();

    /// <summary>
    /// Registers service <see cref="IPluralizationRuleProvider"/>
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddPluralizationRuleProvider(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IPluralizationRuleProvider, PluralizationRuleProvider>();

    /// <summary>
    /// Registers service <see cref="IXmlSchemaValidator"/>
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddXmlSchemaValidator(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IXmlSchemaValidator, XmlSchemaValidator>();
}