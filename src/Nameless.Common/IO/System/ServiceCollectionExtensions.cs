using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.IO.System;

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
        ///     <see cref="IFileProvider"/>.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterFileProvider(IConfiguration? configuration = null) {
            self.ConfigureOptions<FileProviderOptions>(configuration);
            self.TryAddSingleton<FileProviderFactory>();
            self.TryAddSingleton<IFileProvider>(provider => {
                    var options = provider.GetOptions<FileProviderOptions>().Value;

                    return provider.GetRequiredService<FileProviderFactory>()
                                   .GetOrCreate(opts => {
                                       opts.Root = options.Root;
                                       opts.AllowOperationOutsideRoot = options.AllowOperationOutsideRoot;
                                       opts.FileMonitorOptions = options.FileMonitorOptions;
                                   });
                }
            );

            return self;
        }
    }
}
