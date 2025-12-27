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
        ///     Registers the <see cref="IPasswordGenerator"/> service with a
        ///     default implementation.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterPasswordGenerator() {
            self.TryAddSingleton<IPasswordGenerator, RandomPasswordGenerator>();

            return self;
        }

        /// <summary>
        ///     Registers the <see cref="ICrypto"/> service with a
        ///     default implementation.
        /// </summary>
        /// <param name="configure">
        ///     The action to configure the <see cref="CryptoOptions"/> for
        ///     the crypto service.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterCrypto(Action<CryptoOptions>? configure = null) {
            self.Configure(configure ?? (_ => { }));

            return self.InnerRegisterCrypto();
        }

        /// <summary>
        ///     Registers the <see cref="ICrypto"/> service with a
        ///     default implementation.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterCrypto(IConfiguration configuration) {
            var section = configuration.GetSection(nameof(CryptoOptions));

            self.Configure<CryptoOptions>(section);

            return self.InnerRegisterCrypto();
        }

        private IServiceCollection InnerRegisterCrypto() {
            self.TryAddSingleton<ICrypto, RijndaelCrypto>();

            return self;
        }
    }
}