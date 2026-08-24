using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.IO.Explorer;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for File Explorer Provider.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the File Explorer Provider that creates instances of
        ///     <see cref="IFileExplorer"/>.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterFileExplorerProvider() {
            self.TryAddSingleton<IFileExplorerProvider, FileExplorerProvider>();
            self.TryAddSingleton<IFileExplorer>(
                provider => provider.GetRequiredService<IFileExplorerProvider>()
                                    .Create(root: AppDomain.CurrentDomain.BaseDirectory, allowOperationOutsideRoot: false)
            );

            return self;
        }
    }
}
