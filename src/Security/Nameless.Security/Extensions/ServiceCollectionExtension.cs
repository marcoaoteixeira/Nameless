using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;
using Nameless.Security.Options;

namespace Nameless.Security {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection RegisterPasswordGenerator(this IServiceCollection self)
            => self.AddSingleton(RandomPasswordGenerator.Instance);

        public static IServiceCollection RegisterCryptographicService(this IServiceCollection self, Action<RijndaelCryptoOptions>? configure = null)
            => self.AddSingleton<ICryptographicService>(provider => {
                var options = provider.GetPocoOptions<RijndaelCryptoOptions>();

                configure?.Invoke(options);

                return new RijndaelCryptographicService(
                    options: options,
                    logger: provider.GetLogger<RijndaelCryptographicService>()
                );
            });

        #endregion
    }
}
