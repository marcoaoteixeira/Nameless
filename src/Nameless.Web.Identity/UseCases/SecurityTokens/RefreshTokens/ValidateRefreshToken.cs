using Nameless.Mediator.Requests;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.Entities;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

public class
    ValidateRefreshTokenRequestHandler : IRequestHandler<ValidateRefreshTokenRequest, ValidateRefreshTokenResponse> {
    private readonly IUserRefreshTokenManager _userRefreshTokenManager;
    private readonly TimeProvider _timeProvider;

    public ValidateRefreshTokenRequestHandler(IUserRefreshTokenManager userRefreshTokenManager,
        TimeProvider timeProvider) {
        _userRefreshTokenManager = Guard.Against.Null(userRefreshTokenManager);
        _timeProvider = Guard.Against.Null(timeProvider);
    }

    public async Task<ValidateRefreshTokenResponse> HandleAsync(ValidateRefreshTokenRequest request,
        CancellationToken cancellationToken) {
        Guard.Against.Null(request);

        var userRefreshToken = await _userRefreshTokenManager.GetAsync(request.Token, cancellationToken)
                                                             .ConfigureAwait(continueOnCapturedContext: false);
        if (userRefreshToken is null) {
            return new ValidateRefreshTokenResponse();
        }

        var now = _timeProvider.GetUtcNow();
        var status = userRefreshToken.IsActive(now) ? UserRefreshTokenStatus.Active : default;

        // Compose status
        status |= userRefreshToken.IsRevoked() ? UserRefreshTokenStatus.Revoked : default;
        status |= userRefreshToken.IsExpired(now) ? UserRefreshTokenStatus.Expired : default;

        return new ValidateRefreshTokenResponse { UserID = userRefreshToken.UserId, Status = status };
    }
}