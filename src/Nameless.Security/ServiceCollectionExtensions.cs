using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Security;

/// <summary>
///     <see cref="IServiceCollection"/> extensions for security features.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers security services.
        /// </summary>
        /// <param name="configure">
        ///     The action to configure the <see cref="CryptoOptions"/> for
        ///     the crypto service.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterSecurity(Action<CryptoOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterSecurity();
        }

        /// <summary>
        ///     Registers security services.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterSecurity(IConfiguration configuration) {
            var section = configuration.GetSection<CryptoOptions>();

            return self.Configure<CryptoOptions>(section)
                       .InnerRegisterSecurity();
        }

        private IServiceCollection InnerRegisterSecurity() {
            self.TryAddSingleton<IPasswordGenerator, RandomPasswordGenerator>();
            self.TryAddSingleton<ICrypto, RijndaelCrypto>();

            return self;
        }
    }
}