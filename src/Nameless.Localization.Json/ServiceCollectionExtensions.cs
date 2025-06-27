using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers localization services that uses JSON files.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection" />.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> so other actions can be chained.
    /// </returns>
    public static IServiceCollection ConfigureLocalizationServices(this IServiceCollection self, Action<LocalizationOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddSingleton<ICultureProvider, CultureProvider>()
                   .AddSingleton<IResourceManager, ResourceManager>()
                   .AddSingleton<IStringLocalizerFactory, StringLocalizerFactory>()
                   .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
    }
}