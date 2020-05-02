using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Nameless.WebApplication.Auth {
    public interface IJwtTokenHandler {
        #region Methods

        string WriteToken (JwtSecurityToken jwt);
        ClaimsPrincipal ValidateToken (string token, TokenValidationParameters tokenValidationParameters);

        #endregion
    }
}