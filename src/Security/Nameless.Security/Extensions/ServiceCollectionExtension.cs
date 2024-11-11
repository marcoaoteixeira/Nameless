using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;
using Nameless.Security.Options;

namespace Nameless.Security;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddRandomPasswordGenerator(this IServiceCollection self)
        => Prevent.Argument
                  .Null(self)
                  .AddSingleton<IPasswordGenerator, RandomPasswordGenerator>();

    public static IServiceCollection AddRijndaelCryptographicService(this IServiceCollection self, Action<RijndaelCryptoOptions> configure)
        => Prevent.Argument
                  .Null(self)
                  .Configure(configure)
                  .AddSingleton<ICryptographicService, RijndaelCryptographicService>();

    public static IServiceCollection AddRijndaelCryptographicService(this IServiceCollection self, IConfigurationSection rijndaelConfigSection)
        => Prevent.Argument
                  .Null(self)
                  .Configure<RijndaelCryptoOptions>(rijndaelConfigSection)
                  .AddSingleton<ICryptographicService, RijndaelCryptographicService>();
}