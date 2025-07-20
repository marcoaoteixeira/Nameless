using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nameless.Web.Endpoints;
using Nameless.Web.Filters;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Responses;
using Nameless.Web.Identity.Security;
using Nameless.Web.IdentityModel;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Identity.Endpoints;

public record AuthenticateRequest {
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class AuthenticateRequestValidator : AbstractValidator<AuthenticateRequest> {
    public AuthenticateRequestValidator() {
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}

public record AuthenticateResponse {
    public required string AccessToken { get; init; }
    public string? RefreshToken { get; init; }
}

public class Authenticate : IEndpoint {
    private readonly SignInManager<User> _signInManager;
    private readonly IJsonWebTokenProvider _jsonWebTokenProvider;
    private readonly IUserRefreshTokenService _userRefreshTokenService;
    private readonly ILogger<Authenticate> _logger;

    public Authenticate(
        SignInManager<User> signInManager,
        IJsonWebTokenProvider jsonWebTokenProvider,
        IUserRefreshTokenService userRefreshTokenService,
        ILogger<Authenticate> logger) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _jsonWebTokenProvider = Prevent.Argument.Null(jsonWebTokenProvider);
        _userRefreshTokenService = Prevent.Argument.Null(userRefreshTokenService);
        _logger = Prevent.Argument.Null(logger);
    }

    public void Configure(IEndpointDescriptor descriptor) {
        descriptor
            .Post("authenticate", HandleAsync)
            .AllowAnonymous()
            .WithFilter<ValidateRequestEndpointFilter>()
            .Produces<AuthenticateResponse>()
            .ProducesProblem(statusCode: StatusCodes.Status403Forbidden)
            .ProducesProblem(statusCode: StatusCodes.Status404NotFound)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem();
    }

    public async Task<IResult> HandleAsync([FromBody] AuthenticateRequest request, CancellationToken cancellationToken) {
        Prevent.Argument.Null(request);

        var user = await _signInManager.UserManager
                                       .FindByEmailAsync(request.Email)
                                       .ConfigureAwait(continueOnCapturedContext: false);
        if (user is null) {
            _logger.AuthenticateUserNotFound(request.Email);

            return NotFound(new ErrorResponse { Message = "Email or Password invalid." });
        }

        var signinResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true)
                                               .ConfigureAwait(continueOnCapturedContext: false);

        if (signinResult.IsLockedOut) {
            _logger.UserIsLockedOut(request.Email);

            return Forbid();
        }

        if (signinResult.IsNotAllowed) {
            _logger.UserIsNotAllowed(request.Email);

            return Forbid();
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user)
                                            .ConfigureAwait(continueOnCapturedContext: false);
        var jwt = _jsonWebTokenProvider.Create(new JsonWebTokenRequest {
            Claims = principal.Claims
        });

        if (!jwt.Succeeded) {
            _logger.JwtCreationFailed(request.Email, jwt.Error);

            return InternalServerError("Unable to create access token.");
        }

        var refreshToken = await _userRefreshTokenService.CreateAsync(user.Id, cancellationToken)
                                                         .ConfigureAwait(continueOnCapturedContext: false);
        var response = new AuthenticateResponse {
            AccessToken = jwt.Token,
            RefreshToken = refreshToken.Token
        };

        return Ok(response);
    }
}
