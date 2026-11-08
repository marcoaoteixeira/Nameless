using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Configuration;

namespace Nameless.Auth.OAuth;

/// <summary>
///     OAuth Access Token Provider Host Feature
/// </summary>
public sealed class OAuthAccessTokenProviderHostFeature : AccessTokenProviderHostFeature<OAuthTokenRequest> {
    /// <inheritdoc />
    protected override IAccessTokenProvider<OAuthTokenRequest> CreateAccessTokenProvider(IServiceProvider provider, string httpClientName) {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var timeProvider = provider.GetService<TimeProvider>() ?? TimeProvider.System;
        var options = provider.GetRequiredService<IConfiguration>()
                              .GetSectionFor<OAuthClientOptions>()
                              .GetOrThrow<OAuthClientOptions>()
                              .Validate();

        return new OAuthAccessTokenProvider(httpClientFactory, httpClientName, timeProvider, options);
    }
}
