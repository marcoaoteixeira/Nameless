using Microsoft.Extensions.Hosting;
using Nameless.WPF.Documents;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     Document Services Configuration
/// </summary>
public static class DocumentServicesConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the Document Services service.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureDocumentServices(WinHostSettings settings) {
            if (settings.DisableDocumentServices) { return self; }

            self.ConfigureServices(
                services => services.RegisterDocumentServices(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureDocumentServicesRegistration,
                        settings.Assemblies
                    )
                )
            );

            return self;
        }
    }
}
