using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Web.Endpoints;
using Nameless.Web.Filters;
using Nameless.Web.Identity.Endpoints.Accounts.Requests;
using Nameless.Web.Identity.Endpoints.Accounts.Responses;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Nameless.Web.Identity.Endpoints.Accounts;

public sealed class SignIn : EndpointBase {
    private readonly ILogger<SignIn> _logger;
    private readonly IOptions<IdentityOptions> _options;
    private readonly SignInManager<User> _signInManager;

    public override string HttpMethod
        => HttpMethods.POST;

    public override string RoutePattern
        => $"{_options.Value.BaseUrl}{Constants.Endpoints.SIGN_IN}";

    public SignIn(SignInManager<User> signInManager, IOptions<IdentityOptions> options, ILogger<SignIn> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    public override void Configure(IEndpointConfiguration configuration) {
        configuration.WithOpenApi()
               .WithGroupName("Identity/Accounts")
               .WithName("identity_accounts_sign_in")
               .WithDisplayName("Sign In")
               .WithDescription("Sign in the user in the application.")
               .AddEndpointFilter<ValidateEndpointFilter>()
               .AllowAnonymous()
               .Produces<SignInResponse>()
               .Produces<RequiresTwoFactorResponse>()
               .ProducesProblem(StatusCodes.Status401Unauthorized)
               .ProducesProblem(StatusCodes.Status423Locked)
               .ProducesValidationProblem();
    }

    public override Delegate CreateDelegate() {
        return HandleAsync;
    }

    public async Task<IResult> HandleAsync([FromBody] SignInRequest input, CancellationToken cancellation) {
        var signInResult = await _signInManager.PasswordSignInAsync(input.UserName,
            input.Password,
            input.RememberMe,
            _options.Value.LockoutOnFailure);

        if (signInResult.Succeeded) {
            _logger.UserSignInSucceeded();

            return HttpResults.Ok(new SignInResponse { Redirect = input.ReturnUrl });
        }

        if (signInResult.RequiresTwoFactor) {
            return HttpResults.Ok(new RequiresTwoFactorResponse {
                Redirect = $"{_options.Value.BaseUrl}{Constants.Endpoints.TWO_FACTOR_AUTH}",
                RememberMe = input.RememberMe,
                ReturnUrl = input.ReturnUrl
            });
        }

        if (signInResult.IsLockedOut) {
            _logger.SignInUserIsLockedOut();

            return HttpResults.Problem(Constants.Messages.SignIn.USER_LOCKED_OUT_MESSAGE,
                statusCode: StatusCodes.Status423Locked,
                title: Constants.Messages.SignIn.USER_LOCKED_OUT_TITLE);
        }

        return HttpResults.Problem(Constants.Messages.SignIn.INVALID_SIGNIN_ATTEMPT_MESSAGE,
            statusCode: StatusCodes.Status401Unauthorized,
            title: Constants.Messages.SignIn.INVALID_SIGNIN_ATTEMPT_TITLE);
    }
}