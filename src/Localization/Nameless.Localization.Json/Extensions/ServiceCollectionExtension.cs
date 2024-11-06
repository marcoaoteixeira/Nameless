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
    public static IServiceCollection AddJsonLocalization(this IServiceCollection self, Action<LocalizationOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .RegisterLocalizationServices();

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
    /// <param name="localizationOptionsConfigurationSection">The <see cref="LocalizationOptions"/> configuration section.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection self, IConfigurationSection localizationOptionsConfigurationSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<LocalizationOptions>(localizationOptionsConfigurationSection)
                  .RegisterLocalizationServices();

    private static IServiceCollection RegisterLocalizationServices(this IServiceCollection self)
        => self.AddSingleton<ICultureContext, CultureContext>()
               .AddSingleton<ITranslationManager, TranslationManager>()
               .AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>()
               .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
}