using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Compression.Zip;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers Zip Compressor services.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterZipCompressor() {
            self.TryAddSingleton<ICompressor, ZipCompressor>();

            return self;
        }
    }
}
