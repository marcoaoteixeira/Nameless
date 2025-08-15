using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nameless.Mediator;
using Nameless.Mediator.Requests;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Internals;
using Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;
using Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

namespace Nameless.Web.Identity.UseCases.Authentication.SignIn;

public class SignInRequestHandler : IRequestHandler<SignInRequest, SignInResponse> {
    private readonly IMediator _mediator;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<SignInRequestHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="SignInRequestHandler"/> class.
    /// </summary>
    /// <param name="mediator">
    ///     The mediator.
    /// </param>
    /// <param name="signInManager">
    ///     The sign-in manager.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public SignInRequestHandler(
        IMediator mediator,
        SignInManager<User> signInManager,
        ILogger<SignInRequestHandler> logger) {
        _mediator = Guard.Against.Null(mediator);
        _signInManager = Guard.Against.Null(signInManager);
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    public async Task<SignInResponse> HandleAsync(SignInRequest request, CancellationToken cancellationToken) {
        Guard.Against.Null(request);

        var user = await _signInManager.UserManager
                                       .FindByEmailAsync(request.Email)
                                       .ConfigureAwait(continueOnCapturedContext: false);
        if (user is null) {
            _logger.SignInUserNotFound(request.Email);

            return new SignInResponse {
                Error = "Email or Password invalid.",
                ErrorType = SignInErrorType.Invalid
            };
        }

        var signinResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true)
                                               .ConfigureAwait(continueOnCapturedContext: false);

        if (signinResult.IsLockedOut) {
            _logger.SignInUserIsLockedOut(request.Email);

            return new SignInResponse {
                Error = "User is locked out.",
                ErrorType = SignInErrorType.LockedOut
            };
        }

        if (signinResult.IsNotAllowed) {
            _logger.SignInUserIsNotAllowed(request.Email);

            return new SignInResponse {
                Error = "User is not allowed to sign-in.",
                ErrorType = SignInErrorType.NotAllowed
            };
        }

        var accessTokenResponse = await _mediator.ExecuteAsync(new CreateAccessTokenRequest { UserID = user.Id }, cancellationToken);
        var refreshTokenResponse = await _mediator.ExecuteAsync(new CreateRefreshTokenRequest { UserID = user.Id }, cancellationToken);

        var response = new SignInResponse {
            AccessToken = accessTokenResponse.Token,
            RefreshToken = refreshTokenResponse.Token
        };

        return response;
    }
}
