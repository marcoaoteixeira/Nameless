using Microsoft.Extensions.Hosting;
using Nameless.Resilience;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Resilience Configuration
/// </summary>
public static class ResilienceConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Resilience services.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterResilience(WinHostSettings settings) {
            if (settings.DisableResilience) { return self; }

            self.ConfigureServices(
                services => services.RegisterResilience()
            );

            return self;
        }
    }
}
