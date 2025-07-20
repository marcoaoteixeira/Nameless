using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Nameless.Web.IdentityModel;

/// <summary>
///     Default implementation of <see cref="IJsonWebTokenProvider"/>.
/// </summary>
public class JsonWebTokenProvider : IJsonWebTokenProvider {
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<JsonWebTokenOptions> _options;
    private readonly ILogger<JsonWebTokenProvider> _logger;

    /// <summary>
    ///     Initializes a new instance of <see cref="JsonWebTokenProvider"/> class.
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
    public JsonWebTokenResponse Create(JsonWebTokenRequest request) {
        SecurityTokenDescriptor descriptor;

        try { descriptor = CreateSecurityTokenDescriptor(request.Claims); }
        catch (Exception ex) {
            _logger.CreateSecurityTokenDescriptorFailure(ex);

            return new JsonWebTokenResponse { Error = ex.Message };
        }

        try {
            var tokenHandler = new JsonWebTokenHandler();

            // Force JsonWebTokenHandler to use the default claim name
            // https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub
            tokenHandler.InboundClaimTypeMap.Clear();

            var token = tokenHandler.CreateToken(descriptor);

            return new JsonWebTokenResponse {
                Token = token
            };
        }
        catch (Exception ex) {
            _logger.CreateJsonWebTokenFailure(ex);

            return new JsonWebTokenResponse {
                Error = ex.Message
            };
        }
    }

    private SecurityTokenDescriptor CreateSecurityTokenDescriptor(IEnumerable<Claim> claims) {
        var options = _options.Value;
        var innerClaims = claims.ToArray();

        if (string.IsNullOrWhiteSpace(options.Secret)) {
            throw new InvalidOperationException("JSON Web Token secret is not configured.");
        }

        if (innerClaims.All(claim => claim.Type != MS_JwtRegisteredClaimNames.Sub)) {
            throw new InvalidOperationException($"Must provide '{MS_JwtRegisteredClaimNames.Sub}' claim.");
        }

        var result = new SecurityTokenDescriptor {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(options.Secret.GetBytes()),
                algorithm: options.Algorithm ?? SecurityAlgorithms.HmacSha256
            ),
        };

        if (!string.IsNullOrEmpty(options.Issuer)) {
            result.Claims[MS_JwtRegisteredClaimNames.Iss] = options.Issuer;
        }

        if (!string.IsNullOrEmpty(options.Audience)) {
            result.Claims[MS_JwtRegisteredClaimNames.Aud] = options.Audience;
        }

        foreach (var claim in innerClaims) {
            result.Claims[claim.Type] = claim.Value;
        }

        var now = _timeProvider.GetUtcNow();
        var expires = now.Add(options.AccessTokenTtl);
        result.Claims[MS_JwtRegisteredClaimNames.Exp] = $"{expires.ToUnixTimeSeconds()}";
        result.Claims[MS_JwtRegisteredClaimNames.Iat] = $"{now.ToUnixTimeSeconds()}";
        result.Claims[MS_JwtRegisteredClaimNames.Jti] = $"{Guid.NewGuid():N}";

        return result;
    }
}
