using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.IO.Monitoring;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The service collection.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers <see cref="IFileMonitorProvider"/> as a singleton.
        /// </summary>
        /// <returns>
        ///     The same <see cref="IServiceCollection"/> for chaining.
        /// </returns>
        public IServiceCollection RegisterFileMonitorProvider() {
            self.TryAddSingleton<IFileMonitorProvider, FileMonitorProvider>();

            return self;
        }
    }
}
