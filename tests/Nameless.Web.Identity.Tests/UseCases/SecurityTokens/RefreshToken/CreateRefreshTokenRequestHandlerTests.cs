using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Mockers;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Mockers;
using Nameless.Web.Identity.Security;
using Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshToken;

public class CreateRefreshTokenRequestHandlerTests {
    private static CreateRefreshTokenRequestHandler CreateSut(
        IHttpContextAccessor httpContextAccessor = null,
        TimeProvider timeProvider = null,
        UserManager<User> userManager = null,
        IUserRefreshTokenManager userRefreshTokenManager = null,
        IOptions<RefreshTokenOptions> options = null,
        ILogger<CreateRefreshTokenRequestHandler> logger = null) {

        return new CreateRefreshTokenRequestHandler(
            httpContextAccessor ?? new HttpContextAccessorMocker().Build(),
            timeProvider ?? TimeProvider.System,
            userManager ?? IdentityHelpers.CreateUserManager(),
            userRefreshTokenManager ?? new UserRefreshTokenManagerMocker().Build(),
            options ?? IdentityHelpers.CreateOptions<RefreshTokenOptions>(),
            logger ?? NullLogger<CreateRefreshTokenRequestHandler>.Instance
        );
    }

    [Fact]
    public async Task WhenCreatingRefreshToken_WhenRequestIsValid_ThenReturnsRefreshToken() {
        // arrange
        const string ExpectedToken = "08987ff8-ad8e-48b0-be03-5c84411930ec";
        var options = IdentityHelpers.CreateOptions<RefreshTokenOptions>(config => config.UseRefreshToken = true);
        var user = IdentityHelpers.CreateJohnDoeUser();
        var userStore = new UserStoreMocker().WithFindByIdAsync(user)
                                             .Build();
        var userManager = IdentityHelpers.CreateUserManager(userStore);
        var userRefreshToken = new UserRefreshToken {
            Token = ExpectedToken
        };
        var userRefreshTokenManager = new UserRefreshTokenManagerMocker().WithCreateAsync(userRefreshToken)
                                                                         .Build();

        var request = new CreateRefreshTokenRequest { UserID = user.Id };
        var sut = CreateSut(
            userManager: userManager,
            userRefreshTokenManager: userRefreshTokenManager,
            options: options);

        // act
        var response = await sut.HandleAsync(request, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Succeeded);
            Assert.Equal(ExpectedToken, response.Token);
        });
    }

    [Fact]
    public async Task WhenCreatingRefreshToken_WhenOptionsUseRefreshTokenIsFalse_ThenReturnsEmptyRefreshToken() {
        // arrange
        var options = IdentityHelpers.CreateOptions<RefreshTokenOptions>(config => config.UseRefreshToken = false);
        var request = new CreateRefreshTokenRequest { UserID = Guid.NewGuid() };
        var sut = CreateSut(options: options);

        // act
        var response = await sut.HandleAsync(request, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Succeeded);
            Assert.Null(response.Token);
        });
    }
}
