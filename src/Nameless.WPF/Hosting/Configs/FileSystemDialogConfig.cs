using Microsoft.Extensions.Hosting;
using Nameless.WPF.Dialogs.FileSystem;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     File System Dialog Configuration
/// </summary>
public static class FileSystemDialogConfig {
    /// <param name="self">
    ///     The current <see cref="WinHostFactory"/> instance.
    /// </param>
    extension(WinHostBuilder self) {
        /// <summary>
        ///     Configures the File System Dialog services.
        /// </summary>
        /// <param name="settings">
        ///     The settings.
        /// </param>
        /// <returns>
        ///     The current <see cref="WinHostFactory"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public WinHostBuilder ConfigureFileSystemDialog(WinHostSettings settings) {
            if (settings.DisableFileSystemDialog) { return self; }

            self.ConfigureServices(
                services => services.RegisterFileSystemDialog()
            );

            return self;
        }
    }
}
