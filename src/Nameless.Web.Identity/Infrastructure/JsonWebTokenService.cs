using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Security;
using Nameless.Web.IdentityModel.Jwt;

namespace Nameless.Web.Identity.Infrastructure;

public class JsonWebTokenService : IJsonWebTokenService {
    private readonly IJsonWebTokenProvider _jsonWebTokenProvider;
    private readonly IUserRefreshTokenManager _userRefreshTokenManager;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="JsonWebTokenService"/> class.
    /// </summary>
    /// <param name="jsonWebTokenProvider">
    ///     The JSON Web Token provider that generates tokens.
    /// </param>
    /// <param name="userRefreshTokenManager">
    ///     The user refresh token manager that handles refresh tokens.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider that provides the current time.
    /// </param>
    public JsonWebTokenService(IJsonWebTokenProvider jsonWebTokenProvider, IUserRefreshTokenManager userRefreshTokenManager, TimeProvider timeProvider) {
        _jsonWebTokenProvider = Prevent.Argument.Null(jsonWebTokenProvider);
        _userRefreshTokenManager = Prevent.Argument.Null(userRefreshTokenManager);
        _timeProvider = Prevent.Argument.Null(timeProvider);
    }

    /// <inheritdoc />
    public async Task<JsonWebTokenResponse> CreateAsync(Guid userID, CancellationToken cancellationToken) {
        Prevent.Argument.Default(userID);

        var accessToken = _jsonWebTokenProvider.Generate([
            new Claim(JwtRegisteredClaimNames.Sub, userID.ToString())
        ]);

        var refreshToken = await _userRefreshTokenManager.CreateAsync(userID, cancellationToken)
                                                         .ConfigureAwait(continueOnCapturedContext: false);

        return new JsonWebTokenResponse {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <inheritdoc />
    public async Task<JsonWebTokenResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken) {
        Prevent.Argument.NullOrWhiteSpace(refreshToken);

        var userRefreshToken = await _userRefreshTokenManager.GetAsync(refreshToken, cancellationToken)
                                                             .ConfigureAwait(continueOnCapturedContext: false);

        if (userRefreshToken is null || !userRefreshToken.IsActive(_timeProvider.GetUtcNow().DateTime)) {
            throw new InvalidRefreshTokenException();
        }

        return await CreateAsync(userRefreshToken.UserId, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
    }
}