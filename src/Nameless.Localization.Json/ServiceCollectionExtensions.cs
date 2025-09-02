using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Localization.Json.Infrastructure;

namespace Nameless.Localization.Json;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    private const string CULTURE_PROVIDER_KEY = $"{nameof(ICultureProvider)} :: bc2064ca-1195-4bd8-91fd-e726e802183b";
    private const string RESOURCE_PROVIDER_KEY = $"{nameof(IResourceManager)} :: a9265e84-d734-4a96-b7c3-07c226285bc7";

    /// <summary>
    ///     Registers localization services that uses JSON files.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    /// <param name="configure">
    ///     The configuration action.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterJsonLocalization(this IServiceCollection self, Action<JsonLocalizationOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        return self.InnerRegisterJsonLocalization();
    }

    /// <summary>
    ///     Registers localization services that uses JSON files.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection" /> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterJsonLocalization(this IServiceCollection self, IConfiguration configuration) {
        var section = configuration.GetSection(nameof(JsonLocalizationOptions));

        self.Configure<JsonLocalizationOptions>(section);

        return self.InnerRegisterJsonLocalization();
    }

    private static IServiceCollection InnerRegisterJsonLocalization(this IServiceCollection self) {
        self.TryAddKeyedSingleton<ICultureProvider, CultureProvider>(CULTURE_PROVIDER_KEY);
        self.TryAddKeyedSingleton<IResourceManager, ResourceManager>(RESOURCE_PROVIDER_KEY);
        self.TryAddSingleton(ResolveStringLocalizerFactory);
        self.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        return self;
    }

    private static IStringLocalizerFactory ResolveStringLocalizerFactory(IServiceProvider provider) {
        var cultureProvider = provider.GetRequiredKeyedService<ICultureProvider>(CULTURE_PROVIDER_KEY);
        var resourceManager = provider.GetRequiredKeyedService<IResourceManager>(RESOURCE_PROVIDER_KEY);
        var options = provider.GetOptions<JsonLocalizationOptions>();
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new StringLocalizerFactory(cultureProvider, resourceManager, options, loggerFactory);
    }
}
