using System.Security.Claims;

namespace Nameless.Web.IdentityModel.Jwt;

public interface IJsonWebTokenProvider {
    /// <summary>
    ///     Generates a JSON Web Token.
    /// </summary>
    /// <param name="claims">
    ///     The claims to include in the token.
    /// </param>
    /// <returns>
    ///     A <see cref="string"/> containing the generated token.
    /// </returns>
    string Generate(IEnumerable<Claim> claims);
}