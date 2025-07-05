using Microsoft.Extensions.DependencyInjection;

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
        return self.AddSingleton<IPasswordGenerator, RandomPasswordGenerator>();
    }

    /// <summary>
    ///     Registers the <see cref="ICrypto"/> service with a
    ///     default implementation.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    /// <param name="configure">
    ///     The action to configure the <see cref="CryptoOptions"/> for the crypto service.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterCrypto(this IServiceCollection self, Action<CryptoOptions>? configure = null) {
        return self.Configure(configure ?? (_ => { }))
                   .AddSingleton<ICrypto, RijndaelCrypto>();
    }
}