using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Infrastructure;
using Nameless.Infrastructure.Impl;
using Nameless.Services;
using Nameless.Services.Impl;

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
                  .Null(self, nameof(self))
                  .AddSingleton<IApplicationContext>(provider => {
                      var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
                      return new ApplicationContext(hostEnvironment,
                                                    useAppDataSpecialFolder,
                                                    appVersion);
                  });

    /// <summary>
    /// Registers service <see cref="ISystemClock"/>
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
                  .Null(self, nameof(self))
                  .AddSingleton(SystemClock.Instance);

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
        => self.AddSingleton(PluralizationRuleProvider.Instance);

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
        => self.AddSingleton(XmlSchemaValidator.Instance);
}