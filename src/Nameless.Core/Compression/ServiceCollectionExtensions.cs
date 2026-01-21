using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Compression;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the default implementation of the
        ///     <see cref="IZipArchiveService"/> to the service collection
        ///     as a singleton.
        /// </summary>
        /// <returns>
        ///     The <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterZipArchiveService() {
            self.TryAddSingleton<IZipArchiveService, ZipArchiveService>();

            return self;
        }
    }
}
