using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Registration;
using Nameless.Windows.Hosting.Wrappers;
using Nameless.Windows.Localization;
using Microsoft.Extensions.Hosting;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Localization Configuration
/// </summary>
public static class LocalizationConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Localization services.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureLocalizationFeature(WinHostSettings settings) {
            if (settings.DisableLocalization) {
                self.ConfigureServices(
                    services => services.TryAddSingleton(NullLocalizer.Instance)
                );

                return self;
            }

            self.ConfigureServices((ctx, services) => {
                services.RegisterLocalization(
                    registration: AssemblyScanAwareHelper.Join(
                        settings.ConfigureLocalization,
                        settings.Assemblies
                    ),
                    ctx.Configuration
                );
            });

            return self;
        }
    }
}
