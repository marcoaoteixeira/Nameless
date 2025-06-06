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

public sealed class TwoFactorAuth : EndpointBase {
    private readonly ILogger<TwoFactorAuth> _logger;
    private readonly IOptions<IdentityOptions> _options;
    private readonly SignInManager<User> _signInManager;

    public override string HttpMethod
        => HttpMethods.POST;

    public override string RoutePattern
        => $"{_options.Value.BaseUrl}{Constants.Endpoints.TWO_FACTOR_AUTH}";

    public TwoFactorAuth(SignInManager<User> signInManager, IOptions<IdentityOptions> options,
                         ILogger<TwoFactorAuth> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    public override void Configure(IEndpointConfiguration configuration) {
        configuration.WithOpenApi()
               .WithGroupName("Identity/Accounts")
               .WithName("identity_accounts_two_factor_auth")
               .WithDisplayName("Two-Factor Authentication")
               .WithDescription("Validates user using two-factor authentication code.")
               .AddEndpointFilter<ValidateEndpointFilter>()
               .AllowAnonymous()
               .Produces<TwoFactorAuthResponse>()
               .ProducesProblem(StatusCodes.Status400BadRequest)
               .ProducesProblem(StatusCodes.Status401Unauthorized)
               .ProducesProblem(StatusCodes.Status423Locked)
               .ProducesValidationProblem();
    }

    public override Delegate CreateDelegate() {
        return HandleAsync;
    }

    public async Task<IResult> HandleAsync([FromBody] TwoFactorAuthRequest input, CancellationToken cancellation) {
        var currentUser = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (currentUser is null) {
            return HttpResults.Problem(Constants.Messages.TwoFactorAuth.UNABLE_LOAD_USER_MESSAGE,
                statusCode: StatusCodes.Status400BadRequest,
                title: Constants.Messages.TwoFactorAuth.UNABLE_LOAD_USER_TITLE);
        }

        var twoFactorResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(input.TwoFactorCode,
            input.RememberMe,
            input.RememberMachine);

        if (twoFactorResult.Succeeded) {
            _logger.TwoFactorAuthSucceeded(currentUser);

            return HttpResults.Ok(new TwoFactorAuthResponse { Redirect = input.ReturnUrl });
        }

        if (twoFactorResult.IsLockedOut) {
            _logger.TwoFactorUserIsLockedOut(currentUser);

            return HttpResults.Problem(
                string.Format(Constants.Messages.TwoFactorAuth.USER_LOCKED_OUT_MESSAGE, currentUser.Id),
                statusCode: StatusCodes.Status423Locked,
                title: Constants.Messages.TwoFactorAuth.USER_LOCKED_OUT_TITLE);
        }

        _logger.InvalidTwoFactorCode(currentUser);

        return HttpResults.Problem(string.Format(Constants.Messages.TwoFactorAuth.INVALID_CODE_MESSAGE, currentUser.Id),
            statusCode: StatusCodes.Status401Unauthorized,
            title: Constants.Messages.TwoFactorAuth.INVALID_CODE_TITLE);
    }
}