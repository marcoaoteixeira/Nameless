using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Auth.OAuth;
using Nameless.Configuration;
using Nameless.ObjectModel;

namespace Nameless.Auth;

[UnitTest]
public class AuthExtensionsTests {
    public sealed record Req(string Name);
    public sealed record Res(string Token);

    [Fact]
    public void GetToken_ReturnsProviderResult() {
        // arrange
        var provider = new Mock<IAuthorizationTokenProvider<Req, Res>>();
        provider.Setup(p => p.GetTokenAsync(It.IsAny<Req>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Res("abc"));

        // act
        var actual = provider.Object.GetToken(new Req("x"));

        // assert
        Assert.Equal("abc", actual.Token);
    }

    [Fact]
    public void GetToken_WithTimeout_CancelsSlowProvider() {
        // arrange
        var provider = new Mock<IAuthorizationTokenProvider<Req, Res>>();
        provider.Setup(p => p.GetTokenAsync(It.IsAny<Req>(), It.IsAny<CancellationToken>()))
                .Returns<Req, CancellationToken>(async (_, token) => {
                    await Task.Delay(Timeout.Infinite, token);
                    return new Res("never");
                });

        // act & assert
        Assert.ThrowsAny<OperationCanceledException>(() => provider.Object.GetToken(new Req("x"), timeout: 50));
    }

    [Fact]
    public void OAuthAuthorizationTokenResponse_ImplicitOperators_BuildSuccessAndFailures() {
        // arrange
        var token = new OAuthAuthorizationToken { AccessToken = "a" };

        // act
        OAuthAuthorizationTokenResponse ok = token;
        OAuthAuthorizationTokenResponse single = Error.Failure("x");
        OAuthAuthorizationTokenResponse many = new[] { Error.Failure("x"), Error.Failure("y") };

        // assert
        Assert.Multiple(
            () => Assert.Equal("a", ok.Value.AccessToken),
            () => Assert.Single(single.Errors),
            () => Assert.Equal(2, many.Errors.Length)
        );
    }

    [Fact]
    public void RegisterOAuthAuthenticationTokenProvider_WithAuthorityUrl_ResolvesProvider() {
        // arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["OAuth:AuthorityUrl"] = "https://auth.local" })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        // act
        var returned = services.RegisterOAuthAuthenticationTokenProvider(configuration);
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.NotNull(provider.GetRequiredService<IOAuthAuthorizationTokenProvider>())
        );
    }

    [Fact]
    public void RegisterOAuthAuthenticationTokenProvider_WithoutAuthorityUrl_ThrowsMissingConfigurationException() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.RegisterOAuthAuthenticationTokenProvider();
        using var provider = services.BuildServiceProvider();

        // act & assert
        Assert.Throws<MissingConfigurationException>(provider.GetRequiredService<IOAuthAuthorizationTokenProvider>);
    }
}
