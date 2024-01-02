using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.Web.Services {
    public interface IJwtService {
        #region Methods

        string Generate(JwtClaims claims);

        bool Validate(string token, [NotNullWhen(true)] out ClaimsPrincipal? principal);

        #endregion
    }
}
