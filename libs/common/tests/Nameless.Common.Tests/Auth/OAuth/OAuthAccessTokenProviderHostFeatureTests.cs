using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Auth.Support;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Testing.Tools.Mockers.Hosting;
using System.Reflection;

namespace Nameless.Auth.OAuth;

public class OAuthAccessTokenProviderHostFeatureTests {
    private const string TOKEN_JSON = """{"access_token":"tok","token_type":"Bearer","expires_in":3600}""";

    private static IConfigurationManager CreateConfigurationManager(Action<OAuthClientOptions>? configureOptions = null) {
        var options = ActionHelper.FromDelegate(configureOptions);
        var sectionName = ConfigurationSectionNameAttribute.GetSectionName<OAuthClientOptions>();
        var dictionary = typeof(OAuthClientOptions).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                                   .ToDictionary(
                                                       keySelector: prop => $"{sectionName}:{prop.Name}",
                                                       elementSelector: prop => prop.GetValue(options)?.ToString()
                                                   );
        var manager = new ConfigurationManager();

        manager.AddInMemoryCollection(dictionary);

        return manager;
    }

    private static void ConfigureOAuth(OAuthClientOptions o) {
        o.TokenEndpoint = new Uri("https://idp.example.com/oauth/token");
        o.ClientId = "id";
        o.ClientSecret = "secret";
    }

    [Fact]
    public async Task OAuthAccessTokenProviderHostFeature_Register_Cached_Provider() {
        // arrange
        var configuration = CreateConfigurationManager(ConfigureOAuth);
        var idp = StubHttpMessageHandler.RespondingWith(TOKEN_JSON);
        var builder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                        .WithConfiguration(configuration)
                                                        .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration.Build());

        var feature = new OAuthAccessTokenProviderHostFeature();

        feature.Configure(opts => opts.WithHttpClientConfiguration(http => http.ConfigurePrimaryHttpMessageHandler(() => idp)));
        feature.Register(builder, new HostFeatureContext([], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<IAccessTokenProvider<OAuthTokenRequest>>();

        // act
        var first = await sut.GetTokenAsync(new ClientCredentialsRequest("read"), TestContext.Current.CancellationToken);
        var second = await sut.GetTokenAsync(new ClientCredentialsRequest("read"), TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal("tok", first.Value),
            () => Assert.Same(first, second),
            () => Assert.Single(idp.Requests)
        );
    }

    [Fact]
    public void AccessTokenProvider_IsASingleton_SoCacheIsShared() {
        // arrange
        var configuration = CreateConfigurationManager(ConfigureOAuth);
        var builder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                        .WithConfiguration(configuration)
                                                        .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration.Build());

        var feature = new OAuthAccessTokenProviderHostFeature();

        feature.Register(builder, new HostFeatureContext([], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();

        var a = provider.GetRequiredService<IAccessTokenProvider<OAuthTokenRequest>>();
        var b = provider.GetRequiredService<IAccessTokenProvider<OAuthTokenRequest>>();

        Assert.Same(a, b);
    }

    [Fact]
    public void Invalid_OAuthClientOptions_FailAtRegistration_NotAtFirstUse() {
        IConfigurationManager configurationManager = new ConfigurationManager();
        var builder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                        .WithConfiguration(configurationManager)
                                                        .Build();

        builder.Services.AddSingleton<IConfiguration>(configurationManager.Build());

        var feature = new OAuthAccessTokenProviderHostFeature();

        Assert.Throws<ConfigurationBindingException>(
            () => feature.Register(builder, new HostFeatureContext([], NullLogger.Instance))
        );
    }

    [Fact]
    public async Task CachedTokenProvider_AcceptsAnyCustomProvider() {
        // arrange
        var configuration = CreateConfigurationManager(ConfigureOAuth);
        var builder = new HostApplicationBuilderMocker().WithServices(new ServiceCollection())
                                                        .WithConfiguration(configuration)
                                                        .Build();

        builder.Services.AddSingleton<IConfiguration>(configuration.Build());

        var feature = new StringAccessTokenProviderHostFeature();

        feature.Register(builder, new HostFeatureContext([], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<IAccessTokenProvider<string>>();

        // act
        var first = await sut.GetTokenAsync("x", TestContext.Current.CancellationToken);
        var second = await sut.GetTokenAsync("x", TestContext.Current.CancellationToken);

        // assert
        Assert.Same(first, second);
    }

    internal class StringAccessTokenProviderHostFeature : AccessTokenProviderHostFeature<string> {
        protected override IAccessTokenProvider<string> CreateAccessTokenProvider(IServiceProvider provider, string httpClientName) {
            return FakeTokenProvider.Sequential(TimeProvider.System, TimeSpan.FromMinutes(10));
        }
    }
}
