using System.Security.Claims;

namespace Nameless.WebApplication.Auth {
    public interface IJwtTokenValidator {
        #region Methods

        ClaimsPrincipal GetPrincipalFromToken (string token, string signingKey);

        #endregion
    }
}