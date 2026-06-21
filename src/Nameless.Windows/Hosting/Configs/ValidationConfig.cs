using Microsoft.Extensions.Hosting;
using Nameless.Registration;
using Nameless.Validation.FluentValidation;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

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
        public WinHostBuilder ConfigureValidation(WinHostSettings settings) {
            if (settings.DisableValidation) { return self; }

            self.ConfigureServices(services => {
                services.RegisterValidation(
                    AssemblyScanAwareHelper.Join(
                        settings.ConfigureValidationRegistration,
                        settings.Assemblies
                    )
                );
            });

            return self;
        }
    }
}