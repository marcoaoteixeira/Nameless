using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Nameless.AspNetCore.Options;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless.AspNetCore.Services.Impl {
    public sealed class JwtService : IJwtService {
        #region Private Read-Only Fields

        private readonly JwtOptions _options;

        #endregion

        #region Public Properties

        private IClockService? _clock;
        public IClockService Clock {
            get => _clock ??= ClockService.Instance;
            set => _clock = value;
        }

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Public Constructors

        public JwtService(JwtOptions options) {
            _options = Prevent.Against.Null(options, nameof(options));
        }

        #endregion

        #region IJwtService Members

        public string Generate(string userId, string userName, string userEmail) {
            var now = Clock.GetUtcNow();
            var expires = now.AddHours(_options.AccessTokenTtl);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                Claims = new Dictionary<string, object> {
                    // NOTE: Here JwtRegisteredClaimNames.Sub will be substituted by ClaimTypes.NameIdentifier
                    { JwtRegisteredClaimNames.Sub, userId },

                    { JwtRegisteredClaimNames.Exp, expires.ToString() },
                    { JwtRegisteredClaimNames.Iat, now.ToString() },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
                },
                Expires = expires,
                SigningCredentials = new(
                    key: new SymmetricSecurityKey(_options.Secret.GetBytes()),
                    algorithm: SecurityAlgorithms.HmacSha256Signature
                ),
                Subject = new(new Claim[] {
                    new(ClaimTypes.Name, userName),
                    new(ClaimTypes.Email, userEmail)
                })
            };

            if (!string.IsNullOrEmpty(_options.Issuer)) {
                tokenDescriptor.Claims.Add(JwtRegisteredClaimNames.Iss, _options.Issuer);
            }
            if (!string.IsNullOrEmpty(_options.Audience)) {
                tokenDescriptor.Claims.Add(JwtRegisteredClaimNames.Aud, _options.Audience);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool Validate(string token, [NotNullWhen(true)] out ClaimsPrincipal? principal) {
            principal = null;

            try {
                principal = new JwtSecurityTokenHandler()
                    .ValidateToken(
                        token: token,
                        validationParameters: _options.GetTokenValidationParameters(),
                        validatedToken: out var securityToken
                    );

                var validate = securityToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                if (!validate) { principal = null; }

                return validate;
            } catch (Exception ex) { Logger.LogError(ex, ex.Message); }

            return false;
        }

        #endregion
    }
}
