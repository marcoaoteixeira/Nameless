using Microsoft.Extensions.Hosting;
using Nameless.Registration;
using Nameless.Validation.FluentValidation;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Validator Configuration
/// </summary>
public static class ValidatorConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Validator feature.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureValidator(WinHostSettings settings) {
            if (settings.DisableValidator) { return self; }

            self.ConfigureServices(services => services.RegisterValidator(
                AssemblyScanAwareHelper.Join(
                    settings.ConfigureValidator,
                    settings.Assemblies
                )
            ));

            return self;
        }
    }
}