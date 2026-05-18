using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.IO.FileSystem;

namespace Nameless.WPF.Hosting.Configs;

/// <summary>
///     File System Provider Configuration
/// </summary>
public static class FileSystemProviderConfig {
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
        public WinHostBuilder ConfigureFileSystemProvider(WinHostSettings settings) {
            if (settings.DisableFileSystemProvider) { return self; }

            self.ConfigureServices(services => {
                services.ConfigureOptions<FileSystemProviderOptions>();
                services.TryAddSingleton<IFileSystemProvider, FileSystemProvider>();
            });

            return self;
        }
    }
}