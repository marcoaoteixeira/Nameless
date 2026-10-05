using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Compression;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current instance of <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the ZIP compressor feature.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current instance of <see cref="IServiceCollection"/> so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterZipCompressor(IConfiguration? configuration = null) {
            self.ConfigureOptions<ZipCompressorOptions>(configuration);
            self.TryAddSingleton<ICompressor, ZipCompressor>();

            return self;
        }
    }
}
