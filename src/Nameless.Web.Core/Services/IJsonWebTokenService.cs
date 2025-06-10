using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Nameless.Web.Services;

/// <summary>
///     Defines methods to deal with JSON Web Tokens (JWT).
/// </summary>
public interface IJsonWebTokenService {
    string Generate();

    /// <summary>
    /// Tries to validate the JWT and retrieves the associated <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="jwt">The JSON Web Token.</param>
    /// <param name="principal">Output the <see cref="ClaimsPrincipal"/> associated with the token.</param>
    /// <returns>
    /// <see langword="true"/> if is a valid token; otherwise <see langword="false"/>.
    /// </returns>
    bool TryValidate(string jwt, [NotNullWhen(returnValue: true)] out ClaimsPrincipal? principal);
}

/// <summary>
///     Default implementation of <see cref="IJsonWebTokenService"/>.
/// </summary>
public sealed class JsonWebTokenService : IJsonWebTokenService {
    private readonly IOptions<JwtBearerOptions> _jwtBearerOptions;
    private readonly ILogger<JsonWebTokenService> _logger;

    public JsonWebTokenService(IOptions<JwtBearerOptions> jwtBearerOptions, ILogger<JsonWebTokenService> logger) {
        _jwtBearerOptions = Prevent.Argument.Null(jwtBearerOptions);
        _logger = Prevent.Argument.Null(logger);
    }

    public string Generate() {
        return string.Empty;
    }

    /// <inheritdoc />
    public bool TryValidate(string jwt, [NotNullWhen(true)] out ClaimsPrincipal? principal) {
        Prevent.Argument.Null(jwt);

        principal = null;

        try {
            principal = new JwtSecurityTokenHandler()
               .ValidateToken(
                    token: jwt,
                    validationParameters: _jwtBearerOptions.Value.TokenValidationParameters,
                    validatedToken: out var securityToken
                );

            var validate = securityToken is JwtSecurityToken jwtSecurityToken &&
                           jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

            if (!validate) { principal = null; }

            return validate;
        }
        catch (Exception ex) { _logger.JsonWebTokenValidationFailure(ex); }

        return false;
    }
}
