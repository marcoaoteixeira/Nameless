using Microsoft.Extensions.Hosting;
using Nameless.Mediator;

namespace Nameless.WPF.Hosting.Configs;

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
        public WinHostBuilder RegisterMediator(WinHostSettings settings) {
            if (settings.DisableMediator) { return self; }

            self.ConfigureServices(services => {
                services.RegisterMediator(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureMediatorRegistration,
                        settings.Assemblies
                    )
                );
            });

            return self;
        }
    }
}