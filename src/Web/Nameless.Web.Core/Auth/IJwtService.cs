using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.Web.Auth;

public interface IJwtService {
    string Generate(JwtClaims claims);

    bool TryValidate(string token, [NotNullWhen(returnValue: true)] out ClaimsPrincipal? principal);
}