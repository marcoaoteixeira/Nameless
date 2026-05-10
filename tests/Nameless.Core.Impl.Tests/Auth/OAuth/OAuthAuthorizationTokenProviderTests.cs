using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Nameless.Auth;
using Nameless.Auth.OAuth;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Auth.OAuth;

public class OAuthAuthorizationTokenProviderTests {
    private static OAuthAuthorizationTokenProvider CreateSut(HttpMessageHandler handler) {
        var client = new HttpClient(handler) {
            BaseAddress = new Uri("https://auth.example.com")
        };

        var logger = new LoggerMocker<OAuthAuthorizationTokenProvider>()
            .WithAnyLogLevel()
            .Build();

        return new OAuthAuthorizationTokenProvider(client, logger);
    }

    private static OAuthAuthorizationTokenRequest CreateRequest() {
        return new OAuthAuthorizationTokenRequest {
            ClientId = "test-client",
            ClientSecret = "test-secret",
            GrantType = "client_credentials"
        };
    }

    [Fact]
    [UnitTest]
    public async Task GetTokenAsync_SuccessfulResponse_ReturnsToken() {
        // arrange
        var token = new OAuthAuthorizationToken {
            AccessToken = "eyJhbGciOiJSUzI1NiJ9.test",
            TokenType = "Bearer",
            ExpiresIn = 3600
        };

        var json = JsonSerializer.Serialize(token);
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var sut = CreateSut(handlerMock.Object);

        // act
        var response = await sut.GetTokenAsync(CreateRequest(), CancellationToken.None);

        // assert
        Assert.True(response.Success);
        Assert.Equal(token.AccessToken, response.Value.AccessToken);
        Assert.Equal(token.TokenType, response.Value.TokenType);
        Assert.Equal(token.ExpiresIn, response.Value.ExpiresIn);
    }

    [Fact]
    [UnitTest]
    public async Task GetTokenAsync_UnsuccessfulResponse_ReturnsFailure() {
        // arrange
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized) {
                Content = new StringContent("{\"error\":\"invalid_client\"}", Encoding.UTF8, "application/json")
            });

        var sut = CreateSut(handlerMock.Object);

        // act
        var response = await sut.GetTokenAsync(CreateRequest(), CancellationToken.None);

        // assert
        Assert.False(response.Success);
        Assert.NotEmpty(response.Errors);
    }
}
