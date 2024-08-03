﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Microsoft.Json.Infrastructure.Impl;
using Nameless.Localization.Microsoft.Json.Options;

namespace Nameless.Localization.Microsoft.Json {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterLocalization(this IServiceCollection self, Action<LocalizationOptions>? configure = null)
            => self
               .AddSingleton<IStringLocalizerFactory>(provider => {
                   var options = provider.GetOptions<LocalizationOptions>();

                   configure?.Invoke(options.Value);

                   return new StringLocalizerFactory(
                       cultureContext: CultureContext.Instance,
                       translationManager: new TranslationManager(
                           fileProvider: provider.GetRequiredService<IFileProvider>(),
                           options: options.Value
                       )
                   );
               })
               .AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

        #endregion
    }
}
