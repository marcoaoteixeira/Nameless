using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.Web.Services;

/// <summary>
///     Provides services to deal with JWT authentication.
/// </summary>
public interface IJwtService {
    /// <summary>
    ///     Generates a JSON Web Token for the given parameters.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The JSON Web Token.</returns>
    string Generate(JwtParameters parameters);

    /// <summary>
    ///     Tries to validate the provided JSON Web Token.
    /// </summary>
    /// <param name="token">The JSON Web Token</param>
    /// <param name="principal">The output principal associated with the token.</param>
    /// <returns><c>true</c> if is valid; otherwise <c>false</c>.</returns>
    bool TryValidate(string token, [NotNullWhen(true)] out ClaimsPrincipal? principal);
}