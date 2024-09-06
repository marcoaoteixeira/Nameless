using Microsoft.Extensions.Configuration;
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
    public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, bool useAppDataSpecialFolder = false, Version? appVersion = null)
        => Prevent.Argument
                  .Null(self, nameof(self))
                  .AddSingleton<IApplicationContext>(provider => {
                      var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();
                      return new ApplicationContext(hostEnvironment,
                                                    useAppDataSpecialFolder,
                                                    appVersion);
                  });

    /// <summary>
    /// Registers an object to act like an option that will get its values
    /// from <see cref="IConfiguration"/> (using the Bind method).
    /// </summary>
    /// <typeparam name="TOptions">The type of the object.</typeparam>
    /// <param name="self">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/>, so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="configuration"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection RegisterOptions<TOptions>(this IServiceCollection self, IConfiguration configuration)
        where TOptions : class
        => Prevent.Argument
                  .Null(self, nameof(self))
                  .Configure<TOptions>(Prevent.Argument
                                              .Null(configuration, nameof(configuration))
                                              .GetSection(typeof(TOptions).Name));

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
    public static IServiceCollection RegisterClockService(this IServiceCollection self)
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
    public static IServiceCollection RegisterPluralizationRuleProvider(this IServiceCollection self)
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
    public static IServiceCollection RegisterXmlSchemaValidator(this IServiceCollection self)
        => self.AddSingleton(XmlSchemaValidator.Instance);
}