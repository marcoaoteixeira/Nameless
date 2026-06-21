using Microsoft.Extensions.Hosting;
using Nameless.Mediator;
using Nameless.Registration;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Mediator Configuration
/// </summary>
public static class MediatorConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Mediator service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureMediator(WinHostSettings settings) {
            if (settings.DisableMediator) { return self; }

            self.ConfigureServices(services => {
                services.RegisterMediator(
                    AssemblyScanAwareHelper.Join(
                        settings.ConfigureMediatorRegistration,
                        settings.Assemblies
                    )
                );
            });

            return self;
        }
    }
}