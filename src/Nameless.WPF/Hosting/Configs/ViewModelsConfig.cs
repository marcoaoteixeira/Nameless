using Microsoft.Extensions.Hosting;
using Nameless.WPF.Mvvm;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     View Models Configuration
/// </summary>
public static class ViewModelsConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the View Models service.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder RegisterViewModels(WinHostSettings settings) {
            if (settings.DisableViewModels) { return self; }

            self.ConfigureServices(
                services => services.RegisterViewModels(
                    WinHostSettingsHelper.Join(
                        settings.ConfigureViewModelRegistration,
                        settings.Assemblies
                    )
                )
            );

            return self;
        }
    }
}
