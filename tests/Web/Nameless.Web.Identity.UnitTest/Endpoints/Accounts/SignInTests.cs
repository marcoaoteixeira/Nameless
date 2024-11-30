using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Nameless.Mockers;
using Nameless.Web.Identity.Endpoints.Accounts.Requests;
using Nameless.Web.Identity.Endpoints.Accounts.Responses;
using Nameless.Web.Identity.Fakes;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Nameless.Web.Identity.Endpoints.Accounts;

public class SignInTests {
    [Test]
    public async Task WhenSignInWithCorrectCredentials_AndNonTwoFactorAuth_ThenReturnsOkResult() {
        // arrange
        const string returnUrl = "return_url";

        var fakeSignInManager = new FakeSignInManager().SetupPasswordSignInAsync(SignInResult.Success);
        var options = new OptionsMocker<IdentityOptions>().Build();
        var logger = new LoggerMocker<SignIn>().Build();
        var signIn = new SignIn(fakeSignInManager, options, logger);

        // act
        var result = await signIn.HandleAsync(new SignInRequest { ReturnUrl = returnUrl },
                                              CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.InstanceOf<Ok<SignInResponse>>());
            Assert.That(((Ok<SignInResponse>)result).Value, Is.Not.Null);
            Assert.That(((Ok<SignInResponse>)result).Value.Redirect, Is.EqualTo(returnUrl));
        });
    }

    [Test]
    public async Task WhenSignInWithCorrectCredentials_AndTwoFactorAuth_ThenReturnsOkResultForTwoFactorAuthRedirect() {
        // arrange
        const string returnUrl = "return_url";

        var fakeSignInManager = new FakeSignInManager().SetupPasswordSignInAsync(SignInResult.TwoFactorRequired);
        var options = new OptionsMocker<IdentityOptions>().Build();
        var logger = new LoggerMocker<SignIn>().Build();
        var signIn = new SignIn(fakeSignInManager, options, logger);

        // act
        var result = await signIn.HandleAsync(new SignInRequest { ReturnUrl = returnUrl },
                                              CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.InstanceOf<Ok<RequiresTwoFactorResponse>>());
            Assert.That(((Ok<RequiresTwoFactorResponse>)result).Value, Is.Not.Null);
            Assert.That(((Ok<RequiresTwoFactorResponse>)result).Value.ReturnUrl, Is.EqualTo(returnUrl));
            Assert.That(((Ok<RequiresTwoFactorResponse>)result).Value.Redirect.Contains("two-factor-auth"), Is.True);
        });
    }

    [Test]
    public async Task WhenSignInWithCorrectCredentials_ButUserIsLocked_ThenReturnsProblemResultWithLockedStatusCode() {
        // arrange
        var fakeSignInManager = new FakeSignInManager().SetupPasswordSignInAsync(SignInResult.LockedOut);
        var options = new OptionsMocker<IdentityOptions>().Build();
        var loggerMocker = new LoggerMocker<SignIn>().WithLogLevel(LogLevel.Information);
        var signIn = new SignIn(fakeSignInManager, options, loggerMocker.Build());

        // act
        var result = await signIn.HandleAsync(new SignInRequest(), CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Detail, Is.Not.Empty);
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Title, Is.Not.Empty);
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Status, Is.EqualTo(StatusCodes.Status423Locked));

            loggerMocker.VerifyInformationCall(message => message.Contains("locked out"));
        });
    }

    [Test]
    public async Task WhenSignInWithIncorrectCredentials_ThenReturnsProblemsNotAuthorized() {
        // arrange
        var fakeSignInManager = new FakeSignInManager().SetupPasswordSignInAsync(SignInResult.Failed);
        var options = new OptionsMocker<IdentityOptions>().Build();
        var loggerMocker = new LoggerMocker<SignIn>().WithLogLevel(LogLevel.Information);
        var signIn = new SignIn(fakeSignInManager, options, loggerMocker.Build());

        // act
        var result = await signIn.HandleAsync(new SignInRequest(), CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.That(result, Is.InstanceOf<ProblemHttpResult>());
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Detail, Is.Not.Empty);
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Title, Is.Not.Empty);
            Assert.That(((ProblemHttpResult)result).ProblemDetails.Status, Is.EqualTo(StatusCodes.Status401Unauthorized));
        });
    }
}