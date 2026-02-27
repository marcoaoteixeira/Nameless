using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Mediator.Requests;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Infrastructure;
using Nameless.Web.Identity.Internals;
using Nameless.Web.Identity.Security;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public class CreateRefreshTokenRequestHandler : IRequestHandler<CreateRefreshTokenRequest, CreateRefreshTokenResponse> {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeProvider _timeProvider;
    private readonly UserManager<User> _userManager;
    private readonly IUserRefreshTokenManager _userRefreshTokenManager;
    private readonly IOptions<RefreshTokenOptions> _options;
    private readonly ILogger<CreateRefreshTokenRequestHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CreateRefreshTokenRequestHandler"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="userManager">
    ///     The user manager.
    /// </param>
    /// <param name="userRefreshTokenManager">
    ///     The user refresh token manager.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public CreateRefreshTokenRequestHandler(
        IHttpContextAccessor httpContextAccessor,
        TimeProvider timeProvider,
        UserManager<User> userManager,
        IUserRefreshTokenManager userRefreshTokenManager,
        IOptions<RefreshTokenOptions> options,
        ILogger<CreateRefreshTokenRequestHandler> logger) {
        _userRefreshTokenManager = userRefreshTokenManager;
        _userManager = userManager;
        _httpContextAccessor = Throws.When.Null(httpContextAccessor);
        _timeProvider = Throws.When.Null(timeProvider);
        _options = Throws.When.Null(options);
        _logger = Throws.When.Null(logger);
    }

    /// <inheritdoc />
    public async Task<CreateRefreshTokenResponse> HandleAsync(CreateRefreshTokenRequest request,
        CancellationToken cancellationToken) {
        var options = _options.Value;

        if (!options.UseRefreshToken) {
            _logger.RefreshTokenIsDisabled();

            return new CreateRefreshTokenResponse();
        }

        Throws.When.Null(request);

        var user = await _userManager.FindByIdAsync(request.UserID.ToString())
                                     .SkipContextSync();
        if (user is null) {
            throw new UserNotFoundException();
        }

        var userRefreshToken = await _userRefreshTokenManager.CreateAsync(user, cancellationToken)
                                                             .SkipContextSync();

        var cleanUpCount = await _userRefreshTokenManager.CleanUpAsync(user, cancellationToken)
                                                         .SkipContextSync();

        var revokeMetadata = new RevokeUserRefreshTokenMetadata(_timeProvider.GetUtcNow(),
            _httpContextAccessor.HttpContext?.GetIPv4(), RevokeReason: "Creating new user refresh token.",
            userRefreshToken.Id.ToString());

        var revokeCount = await _userRefreshTokenManager.RevokeAsync(user, revokeMetadata, cancellationToken)
                                                        .SkipContextSync();

        return new CreateRefreshTokenResponse { Token = userRefreshToken.Token };
    }
}