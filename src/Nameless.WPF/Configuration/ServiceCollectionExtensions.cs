using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.WPF.Configuration.Impl;

namespace Nameless.WPF.Configuration;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the <see cref="IAppConfigurationManager"/> service.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterAppConfigurationManager() {
            self.TryAddSingleton<IAppConfigurationManager, AppConfigurationManager>();

            return self;
        }
    }
}
