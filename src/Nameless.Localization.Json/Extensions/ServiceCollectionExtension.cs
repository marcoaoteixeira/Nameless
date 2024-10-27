using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Infrastructure.Impl;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json;

public static class ServiceCollectionExtension {
    private const string CULTURE_CONTEXT_KEY = $"{nameof(CultureContext)}::CC8EC505-F2E0-4ED7-B5B3-58EAD3D47E7B";
    private const string TRANSLATION_MANAGER_KEY = $"{nameof(TranslationManager)}::D94FE075-B906-430D-8FF4-FD41CAF9643E";

    /// <summary>
    /// Registers localization service based on JSON files.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddJsonLocalization(this IServiceCollection self, Action<LocalizationOptions>? configure = null)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure ?? (_ => { }))
                  .AddKeyedSingleton<ICultureContext, CultureContext>(CULTURE_CONTEXT_KEY)
                  .AddKeyedSingleton<ITranslationManager, TranslationManager>(TRANSLATION_MANAGER_KEY)
                  .AddSingleton<IStringLocalizerFactory>(provider => new StringLocalizerFactory(cultureContext: provider.GetRequiredKeyedService<ICultureContext>(CULTURE_CONTEXT_KEY),
                                                                                                translationManager: provider.GetRequiredKeyedService<ITranslationManager>(TRANSLATION_MANAGER_KEY),
                                                                                                loggerFactory: provider.GetRequiredService<ILoggerFactory>()))
                  .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
}