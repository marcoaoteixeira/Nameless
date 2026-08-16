using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Nameless.Helpers;

namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods for
///     <see cref="SmartFileSystemWatcher"/>.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="services">
    /// The service collection.
    /// </param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers <see cref="IFileSystemWatcher"/> as a singleton
        ///     backed by <see cref="SmartFileSystemWatcher"/>.
        /// </summary>
        /// <remarks>
        ///     <see cref="IFileProvider"/> must be separately registered in
        ///     the container. In production use a <c>PhysicalFileProvider</c>
        ///     pointing to the desired directory.
        /// </remarks>
        /// <param name="configure">
        ///     Delegate to configure
        ///     <see cref="SmartFileSystemWatcherOptions"/>.
        /// </param>
        /// <returns>
        ///     The same <see cref="IServiceCollection"/> for chaining.
        /// </returns>
        public IServiceCollection RegisterSmartFileSystemWatcher(Action<SmartFileSystemWatcherOptions>? configure = null) {
            var options = ActionHelper.FromDelegate(configure);

            services.ConfigureOptions(options);
            services.TryAddSingleton<IFileSystemWatcher, SmartFileSystemWatcher>();

            return services;
        }
    }
}
