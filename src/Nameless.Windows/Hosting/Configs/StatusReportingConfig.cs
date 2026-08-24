using Microsoft.Extensions.Hosting;
using Nameless.Registration;
using Nameless.Reporting;
using Nameless.Validation.FluentValidation;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Status Reporting Configuration
/// </summary>
public static class StatusReportingConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Status Reporting feature.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureStatusReporting(WinHostSettings settings) {
            if (settings.DisableStatusReporting) { return self; }

            self.ConfigureServices(services => services.RegisterStatusReporting(
                AssemblyScanAwareHelper.Join(
                    settings.ConfigureStatusReporting,
                    settings.Assemblies
                )
            ));

            return self;
        }
    }
}