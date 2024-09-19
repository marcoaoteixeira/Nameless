using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Json.Infrastructure.Impl;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json;

public static class ServiceCollectionExtension {
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
                  .Null(self, nameof(self))
                  .AddSingleton<IStringLocalizerFactory>(provider => {
                      var options = provider.GetOptions<LocalizationOptions>();
                      
                      configure?.Invoke(options.Value);

                      var cultureContext = CultureContext.Instance;
                      var translationManager = new TranslationManager(fileProvider: provider.GetRequiredService<IFileProvider>(),
                                                                      options: options,
                                                                      logger: provider.GetLogger<TranslationManager>());
                      
                      return new StringLocalizerFactory(cultureContext, translationManager);
                  })
                  .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
}