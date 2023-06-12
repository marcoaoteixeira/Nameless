using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nameless.Services;
using Nameless.Services.Impl;
using Nameless.WebApplication.Options;

namespace Nameless.WebApplication.Services.Impl {

    public sealed class AccessTokenService : IAccessTokenService {

        #region Private Read-Only Fields

        private readonly JsonWebTokenOptions _options;

        #endregion

        #region Public Properties

        private IClock _clock = default!;
        public IClock Clock {
            get { return _clock ??= DefaultClock.Instance; }
            set { _clock = value ?? DefaultClock.Instance; }
        }

        #endregion

        #region Public Constructors

        public AccessTokenService(IOptions<JsonWebTokenOptions> options) {
            Prevent.Null(options, nameof(options));

            _options = options.Value ?? JsonWebTokenOptions.Default;
        }

        #endregion

        #region ITokenService Members

        public Task<string> GenerateAsync(Guid userId, string userName, string userEmail, CancellationToken cancellationToken = default) {
            var issuer = _options.Issuer;
            var audience = _options.Audience;
            var now = Clock.UtcNow;
            var expires = now.AddSeconds(_options.AccessTokenTtl);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Issuer = issuer,
                Audience = audience,
                Claims = new Dictionary<string, object> {
                    // NOTE: Here JwtRegisteredClaimNames.Sub will be substituted by ClaimTypes.NameIdentifier
                    { JwtRegisteredClaimNames.Sub, userId.ToString() },

                    { JwtRegisteredClaimNames.Iss, issuer },
                    { JwtRegisteredClaimNames.Exp, expires.ToString() },
                    { JwtRegisteredClaimNames.Iat, now.ToString() },
                    { JwtRegisteredClaimNames.Aud, audience },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() }
                },
                Expires = expires,
                SigningCredentials = new(
                    key: new SymmetricSecurityKey(_options.Secret.GetBytes()),
                    algorithm: SecurityAlgorithms.HmacSha256Signature
                ),
                Subject = new(new Claim[] {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, userEmail)
                })
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        public Task<ClaimsPrincipal> ExtractAsync(string token, CancellationToken cancellationToken = default) {
            var principal = new JwtSecurityTokenHandler().ValidateToken(
                token: token,
                validationParameters: _options.GetTokenValidationParameters(),
                validatedToken: out SecurityToken securityToken
            );

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token");
            }

            return Task.FromResult(principal);
        }

        #endregion
    }
}
