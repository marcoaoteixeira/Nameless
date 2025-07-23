using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Nameless.Web.IdentityModel.Jwt;

/// <summary>
///     Default implementation of <see cref="IJsonWebTokenProvider"/>.
/// </summary>
public class JsonWebTokenProvider : IJsonWebTokenProvider {
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<JsonWebTokenOptions> _options;
    private readonly ILogger<JsonWebTokenProvider> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="JsonWebTokenProvider"/>
    ///     class.
    /// </summary>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public JsonWebTokenProvider(TimeProvider timeProvider, IOptions<JsonWebTokenOptions> options, ILogger<JsonWebTokenProvider> logger) {
        _timeProvider = Prevent.Argument.Null(timeProvider);
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
    }

    /// <inheritdoc />
    /// <exception cref="MissingSecretConfigurationException">
    ///     if the JWT secret is not configured.
    /// </exception>
    /// <exception cref="MissingClaimSubException">
    ///     if the 'sub' claim is missing from the provided claims.
    /// </exception>
    public string Generate(IEnumerable<Claim> claims) {
        try {
            var descriptor = CreateSecurityTokenDescriptor(claims);
            var tokenHandler = new JsonWebTokenHandler();

            // Force JsonWebTokenHandler to use the default claim name
            // https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub
            tokenHandler.InboundClaimTypeMap.Clear();

            return tokenHandler.CreateToken(descriptor);
        }
        catch (MissingSecretConfigurationException) { _logger.MissingSecretConfiguration(); throw; }
        catch (MissingClaimSubException) { _logger.MissingClaimSub(); throw; }
        catch (Exception ex) { _logger.CreateJsonWebTokenFailure(ex); throw; }
    }

    private SecurityTokenDescriptor CreateSecurityTokenDescriptor(IEnumerable<Claim> claims) {
        var options = _options.Value;

        if (string.IsNullOrWhiteSpace(options.Secret)) {
            throw new MissingSecretConfigurationException();
        }

        var innerClaims = claims.ToArray();
        if (innerClaims.All(claim => claim.Type != MS_JwtRegisteredClaimNames.Sub)) {
            throw new MissingClaimSubException();
        }

        var result = new SecurityTokenDescriptor {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(options.Secret.GetBytes()),
                algorithm: options.SecurityAlgorithm ?? SecurityAlgorithms.HmacSha256
            ),
        };

        if (!string.IsNullOrEmpty(options.Issuer)) {
            result.Claims[MS_JwtRegisteredClaimNames.Iss] = options.Issuer;
        }

        if (options.Audiences.Length > 0) {
            foreach (var audience in options.Audiences) {
                result.Audiences.Add(audience);
            }
        }

        foreach (var additionalClaim in options.AdditionalClaims) {
            result.Claims[additionalClaim.Type] = additionalClaim.Value;
        }

        foreach (var claim in innerClaims) {
            result.Claims[claim.Type] = claim.Value;
        }

        var now = _timeProvider.GetUtcNow();
        result.Claims[MS_JwtRegisteredClaimNames.Iat] = $"{now.ToUnixTimeSeconds()}";

        var expires = now.Add(options.TokenExpiresIn);
        result.Claims[MS_JwtRegisteredClaimNames.Exp] = $"{expires.ToUnixTimeSeconds()}";

        result.Claims[MS_JwtRegisteredClaimNames.Jti] = $"{Guid.NewGuid():N}";

        return result;
    }
}
