using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Attributes;
using Nameless.Configuration;

namespace Nameless.Auth.OAuth;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
        public IServiceCollection RegisterOAuthAuthenticationTokenProvider(IConfiguration configuration) {
            self.ConfigureOptions<OAuthOptions>(configuration);
            self.AddHttpClient<IOAuthAuthorizationTokenProvider, OAuthAuthorizationTokenProvider>((provider, client) => {
                var opts = provider.GetOptions<OAuthOptions>().Value;

                if (string.IsNullOrWhiteSpace(opts.AuthorityUrl)) {
                    throw new MissingConfigurationException(
                        section: ConfigurationSectionNameAttribute.GetSectionName<OAuthOptions>(),
                        key: nameof(OAuthOptions.AuthorityUrl)
                    );
                }

                client.BaseAddress = new Uri(opts.AuthorityUrl);
            });

            return self;
        }
    }
}
