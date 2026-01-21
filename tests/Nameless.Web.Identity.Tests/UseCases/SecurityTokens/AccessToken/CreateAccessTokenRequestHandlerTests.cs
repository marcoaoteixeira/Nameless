using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Jwt;
using Nameless.Web.Identity.Mockers;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;

[UnitTest]
public class CreateAccessTokenRequestHandlerTests {
    private static CreateAccessTokenRequestHandler CreateSut(
        SignInManager<User> signInManager = null,
        TimeProvider timeProvider = null,
        IOptions<JsonWebTokenOptions> options = null,
        ILogger<CreateAccessTokenRequestHandler> logger = null) {
        return new CreateAccessTokenRequestHandler(
            signInManager ?? IdentityHelpers.CreateSignInManager(),
            timeProvider ?? TimeProvider.System,
            options ?? IdentityHelpers.CreateOptions<JsonWebTokenOptions>(),
            logger ?? IdentityHelpers.CreateLogger<CreateAccessTokenRequestHandler>()
        );
    }

    [Fact]
    public async Task WhenCreatingAccessToken_WhenRequestIsValid_ThenReturnsAccessToken() {
        // arrange
        var user = IdentityHelpers.CreateJohnDoeUser();
        var sub = new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());
        var userStore = new UserStoreMocker().WithFindByIdAsync(user)
                                             .WithGetUserIdAsync(user.Id.ToString())
                                             .WithGetUserNameAsync(user.Email)
                                             .WithGetSecurityStampAsync(returnValue: "SecurityStamp")
                                             .WithGetClaimsAsync([sub])
                                             .Build();
        var userManager = IdentityHelpers.CreateUserManager(userStore);
        var signInManager = IdentityHelpers.CreateSignInManager(
            userManager
        );
        var options = IdentityHelpers.CreateOptions<JsonWebTokenOptions>(opts => {
            opts.Secret = "312a13d5-607c-4814-8c82-9ad995a770a8";
        });
        var request = new CreateAccessTokenRequest { UserID = user.Id };
        var sut = CreateSut(signInManager, options: options);

        // act
        var response = await sut.HandleAsync(request, CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.NotNull(response.Token);
        });
    }
}