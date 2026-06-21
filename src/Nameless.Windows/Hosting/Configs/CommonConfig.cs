using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Application;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     Common Configuration
/// </summary>
public static class CommonConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the common services.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureCommonServices() {
            self.ConfigureServices((ctx, services) => {
                services.AddOptions();
                services.TryAddSingleton(TimeProvider.System);
                services.RegisterApplicationContext(ctx.Configuration);
            });

            return self;
        }
    }
}
