using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Auth.OAuth;

namespace Nameless.Auth;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the OAuth <see cref="IAuthorizationTokenProvider{TRequest,TToken}"/>
        ///     implementation.
        /// </summary>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     action can be chained.
        /// </returns>
        public IServiceCollection RegisterAuthenticationTokenProvider(IConfiguration configuration) {
            self.ConfigureOptions<OAuthOptions>(configuration);
            self.AddHttpClient<IAuthorizationTokenProvider<OAuthAuthorizationTokenRequest, OAuthAuthorizationToken>, OAuthAuthorizationTokenProvider>((provider, client) => {
                var opts = provider.GetOptions<OAuthOptions>().Value;

                client.BaseAddress = new Uri(opts.AuthorityUrl);

                foreach (var kvp in opts.Header) {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        kvp.Key,
                        kvp.Value
                    );
                }
            });

            return self;
        }
    }
}
