using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Infrastructure;

public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection" />.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers <see cref="IApplicationContext" /> implementation in the
        ///     service collection.
        /// </summary>
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" />, so other actions can
        ///     be chained.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="self"/> is <see langword="null"/>.
        /// </exception>
        public IServiceCollection RegisterApplicationContext(Action<ApplicationContextOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterApplicationContext();
        }

        /// <summary>
        ///     Registers <see cref="IApplicationContext" /> implementation in the
        ///     service collection.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection" />, so other actions can
        ///     be chained.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if
        ///         <paramref name="self"/> or
        ///         <paramref name="configuration"/>
        ///     is <see langword="null"/>.
        /// </exception>
        public IServiceCollection RegisterApplicationContext(IConfiguration configuration) {
            var section = configuration.GetSection(nameof(ApplicationContextOptions));

            return self.Configure<ApplicationContextOptions>(section)
                       .InnerRegisterApplicationContext();
        }

        private IServiceCollection InnerRegisterApplicationContext() {
            self.TryAddSingleton<IApplicationContext, ApplicationContext>();

            return self;
        }
    }
}