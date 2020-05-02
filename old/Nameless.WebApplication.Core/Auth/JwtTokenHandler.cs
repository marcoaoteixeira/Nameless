using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Nameless.Logging;

namespace Nameless.WebApplication.Auth {
    public sealed class JwtTokenHandler : IJwtTokenHandler {
        #region Private Read-Only Fields

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public JwtTokenHandler () {
            if (_jwtSecurityTokenHandler == null) {
                _jwtSecurityTokenHandler = new JwtSecurityTokenHandler ();
            }
        }

        #endregion

        #region IJwtTokenHandler Members

        public string WriteToken (JwtSecurityToken token) {
            return _jwtSecurityTokenHandler.WriteToken (token);
        }

        public ClaimsPrincipal ValidateToken (string token, TokenValidationParameters tokenValidationParameters) {
            try {
                var principal = _jwtSecurityTokenHandler.ValidateToken (token, tokenValidationParameters, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                    throw new SecurityTokenException ("Invalid token");
                }

                return principal;
            } catch (Exception ex) { _logger.Error ($"Token validation failed: {ex.Message}"); return null; }
        }

        #endregion
    }
}