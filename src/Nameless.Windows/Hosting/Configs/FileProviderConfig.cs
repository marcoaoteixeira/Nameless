using Microsoft.Extensions.Hosting;
using Nameless.IO.System;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Configs;

/// <summary>
///     File System Provider Configuration
/// </summary>
public static class FileProviderConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the File System Provider service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WinHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureFileExplorerFeature(WinHostSettings settings) {
            if (settings.DisableFileExplorer) { return self; }
            
            self.ConfigureServices(
                services => services.RegisterFileProvider()
            );

            return self;
        }
    }
}