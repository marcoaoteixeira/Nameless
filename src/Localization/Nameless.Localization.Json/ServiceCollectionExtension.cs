using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Registers localization service based on JSON files.
    /// </summary>
    /// <remarks>
    /// This services depends on:
    /// <list type="bullet">
    ///     <item>
    ///         <description><see cref="Microsoft.Extensions.FileProviders.IFileProvider"/></description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="Microsoft.Extensions.Logging.ILogger{TCategory}"/></description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection RegisterJsonLocalizationServices(this IServiceCollection self, Action<LocalizationOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterServices();

    /// <summary>
    /// Registers localization service based on JSON files.
    /// </summary>
    /// <remarks>
    /// This services depends on:
    /// <list type="bullet">
    ///     <item>
    ///         <description><see cref="Microsoft.Extensions.FileProviders.IFileProvider"/></description>
    ///     </item>
    ///     <item>
    ///         <description><see cref="Microsoft.Extensions.Logging.ILogger{TCategory}"/></description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="localizationConfigSection">The <see cref="LocalizationOptions"/> configuration section.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection RegisterJsonLocalizationServices(this IServiceCollection self, IConfigurationSection localizationConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<LocalizationOptions>(localizationConfigSection)
                  .RegisterServices();

    private static IServiceCollection RegisterServices(this IServiceCollection self)
        => self.AddSingleton<ICultureProvider, CultureProvider>()
               .AddSingleton<IResourceManager, ResourceManager>()
               .AddSingleton<IPluralizationRuleProvider, PluralizationRuleProvider>()
               .AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>()
               .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
}