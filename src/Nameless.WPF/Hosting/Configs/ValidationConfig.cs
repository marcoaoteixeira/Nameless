using Microsoft.Extensions.Hosting;
using Nameless.Validation.FluentValidation;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Validation Configuration
/// </summary>
public static class ValidationConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Validation service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterValidation(WinHostSettings settings) {
            if (settings.DisableValidation) { return self; }

            self.ConfigureServices(services => {
                services.RegisterValidation(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureValidationRegistration,
                        settings.Assemblies
                    )
                );
            });

            return self;
        }
    }
}