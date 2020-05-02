using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Nameless.WebApplication.Auth {
    public sealed class JwtTokenValidator : IJwtTokenValidator {
        #region Private Read-Only Fields

        private readonly IJwtTokenHandler _jwtTokenHandler;

        #endregion

        #region Public Constructors

        public JwtTokenValidator (IJwtTokenHandler jwtTokenHandler) {
            Prevent.ParameterNull (jwtTokenHandler, nameof (jwtTokenHandler));
            
            _jwtTokenHandler = jwtTokenHandler;
        }

        #endregion

        #region IJwtTokenValidator Members

        public ClaimsPrincipal GetPrincipalFromToken (string token, string signingKey) {
            return _jwtTokenHandler.ValidateToken (token, new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (signingKey)),
                ValidateLifetime = false // we check expired tokens here
            });
        }

        #endregion
    }
}