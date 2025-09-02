using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Security;

/// <summary>
///     <see cref="IServiceCollection"/> extensions for security features.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    ///     Registers the <see cref="IPasswordGenerator"/> service with a
    ///     default implementation.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterPasswordGenerator(this IServiceCollection self) {
        self.TryAddSingleton<IPasswordGenerator, RandomPasswordGenerator>();

        return self;
    }

    /// <summary>
    ///     Registers the <see cref="ICrypto"/> service with a
    ///     default implementation.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <param name="configure">
    ///     The action to configure the <see cref="CryptoOptions"/> for
    ///     the crypto service.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterCrypto(this IServiceCollection self, Action<CryptoOptions>? configure = null) {
        self.Configure(configure ?? (_ => { }));

        return self.InnerRegisterCrypto();
    }

    /// <summary>
    ///     Registers the <see cref="ICrypto"/> service with a
    ///     default implementation.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <param name="configuration">
    ///     The configuration.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions
    ///     can be chained.
    /// </returns>
    public static IServiceCollection RegisterCrypto(this IServiceCollection self, IConfiguration configuration) {
        var section = configuration.GetSection(nameof(CryptoOptions));

        self.Configure<CryptoOptions>(section);

        return self.InnerRegisterCrypto();
    }

    private static IServiceCollection InnerRegisterCrypto(this IServiceCollection self) {
        self.TryAddSingleton<ICrypto, RijndaelCrypto>();

        return self;
    }
}