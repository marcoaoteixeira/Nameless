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

    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers localization services that uses JSON files.
        /// </summary>
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLocalization(Action<JsonLocalizationOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterLocalization();
        }

        /// <summary>
        ///     Registers localization services that uses JSON files.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" /> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLocalization(IConfiguration configuration) {
            var section = configuration.GetSection<JsonLocalizationOptions>();

            return self.Configure<JsonLocalizationOptions>(section)
                       .InnerRegisterLocalization();
        }

        private IServiceCollection InnerRegisterLocalization() {
            self.TryAddKeyedSingleton<ICultureProvider, CultureProvider>(CULTURE_PROVIDER_KEY);
            self.TryAddKeyedSingleton<IResourceManager, ResourceManager>(RESOURCE_PROVIDER_KEY);

            self.Replace(ServiceDescriptor.Singleton<IStringLocalizerFactory>(ResolveStringLocalizerFactory));
            self.Replace(ServiceDescriptor.Transient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>)));

            return self;
        }
    }

    private static StringLocalizerFactory ResolveStringLocalizerFactory(IServiceProvider provider) {
        var cultureProvider = provider.GetRequiredKeyedService<ICultureProvider>(CULTURE_PROVIDER_KEY);
        var resourceManager = provider.GetRequiredKeyedService<IResourceManager>(RESOURCE_PROVIDER_KEY);
        var options = provider.GetOptions<JsonLocalizationOptions>();
        var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;

        return new StringLocalizerFactory(cultureProvider, resourceManager, options, loggerFactory);
    }
}