using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Auth;
using Nameless.Auth.OAuth;
using Nameless.Configuration;
using Nameless.Helpers;

namespace Nameless.Hosting.Features.Auth;

/// <summary>
///     Access Token Provider Host Feature
/// </summary>
public abstract class AccessTokenProviderHostFeature<TRequest> : HostFeature<AccessTokenProviderFeatureOptions<TRequest>> where TRequest : notnull {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        // The options are read at registration time so configuration errors show up at startup.
        _ = builder.Configuration
                   .GetSectionFor<OAuthClientOptions>()
                   .GetOrThrow<OAuthClientOptions>()
                   .Validate();

        var featureOptions = ActionHelper.FromDelegate(ConfigureFeature);
        var httpClientName = $"token-source:{typeof(TRequest).Name}";

        builder.Services.TryAddSingleton(TimeProvider.System);

        builder.Services.TryAddSingleton<IAccessTokenProvider<TRequest>>(provider => {
            var timeProvider = provider.GetRequiredService<TimeProvider>();
            var options = provider.GetRequiredService<IConfiguration>()
                                  .GetSectionFor<OAuthClientOptions>()
                                  .GetOrThrow<OAuthClientOptions>();

            var accessTokenProvider = CreateAccessTokenProvider(provider, httpClientName);

            return featureOptions.UseCache
                ? new CachingAccessTokenProvider<TRequest>(accessTokenProvider, timeProvider, options.ClockSkew, featureOptions.RequestComparer)
                : accessTokenProvider;
        });

        var httpClientBuilder = builder.Services.AddHttpClient(httpClientName);

        featureOptions.ConfigureHttpClient?.Invoke(
            httpClientBuilder
        );
    }

    /// <summary>
    ///     Creates the <see cref="IAccessTokenProvider{TRequest}"/> implementation instance.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    /// <param name="httpClientName">
    ///     The HTTP client name defined for this provider.
    /// </param>
    /// <returns>
    ///     A new instance of <see cref="IAccessTokenProvider{TRequest}"/> class.
    /// </returns>
    protected abstract IAccessTokenProvider<TRequest> CreateAccessTokenProvider(IServiceProvider provider, string httpClientName);
}
