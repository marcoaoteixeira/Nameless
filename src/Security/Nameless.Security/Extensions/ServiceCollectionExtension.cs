using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;
using Nameless.Security.Options;

namespace Nameless.Security;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddPasswordGenerator(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton(RandomPasswordGenerator.Instance);

    public static IServiceCollection AddCryptographicService(this IServiceCollection self, Action<RijndaelCryptoOptions>? configure = null)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<ICryptographicService>(provider => {
                      var options = provider.GetOptions<RijndaelCryptoOptions>();

                      configure?.Invoke(options.Value);

                      return new RijndaelCryptographicService(options: options,
                                                              logger: provider.GetLogger<RijndaelCryptographicService>());
                  });
}