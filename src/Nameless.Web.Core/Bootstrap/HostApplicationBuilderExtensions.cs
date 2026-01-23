using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap;

namespace Nameless.Web.Bootstrap;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <typeparamref name="THostApplicationBuilder"/>.
    /// </param>
    /// <typeparam name="THostApplicationBuilder">
    ///     Type that implements <see cref="IHostApplicationBuilder"/>.
    /// </typeparam>
    extension<THostApplicationBuilder>(THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {
        /// <summary>
        ///     Registers the bootstrap services and steps.
        /// </summary>
        /// <param name="configure">The configuration action.</param>
        /// <returns>
        ///     The current <typeparamref name="THostApplicationBuilder"/>
        ///     instance so other actions can be chained.
        /// </returns>
        public THostApplicationBuilder RegisterBootstrap(Action<BootstrapOptions>? configure = null) {
            self.Services.RegisterBootstrap(configure);

            return self;
        }
    }
}