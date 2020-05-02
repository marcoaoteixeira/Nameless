using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Nameless.WebApplication.Auth {
    public sealed class JwtFactory : IJwtFactory {
        #region Private Read-Only Fields

        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtIssuerOptions;

        #endregion

        public JwtFactory (IJwtTokenHandler jwtTokenHandler, JwtIssuerOptions jwtIssuerOptions) {
            Prevent.ParameterNull (jwtTokenHandler, nameof (jwtTokenHandler));
            Prevent.ParameterNull (jwtIssuerOptions, nameof (jwtIssuerOptions));

            _jwtTokenHandler = jwtTokenHandler;
            _jwtIssuerOptions = jwtIssuerOptions;

            ValidateOptions (_jwtIssuerOptions);
        }

        #region Private Static Methods

        private static ClaimsIdentity GenerateClaimsIdentity (string id, string userName) {
            return new ClaimsIdentity (new GenericIdentity (userName, "Token"), new [] {
                new Claim (Constants.JwtClaimIdentifiers_Id, id),
                new Claim (Constants.JwtClaimIdentifiers_Rol, Constants.JwtClaims_ApiAccess)
            });
        }

        private static void ValidateOptions (JwtIssuerOptions opts) {
            if (opts.ValidFor <= TimeSpan.Zero) {
                throw new ArgumentException ("Must be a non-zero TimeSpan.", nameof (JwtIssuerOptions.ValidFor));
            }

            if (opts.SigningCredentials == null) {
                throw new ArgumentNullException (nameof (JwtIssuerOptions.SigningCredentials));
            }

            if (opts.JtiGenerator == null) {
                throw new ArgumentNullException (nameof (JwtIssuerOptions.JtiGenerator));
            }
        }

        #endregion

        #region IJwtFactory Members

        public async Task<AccessToken> GenerateEncodedToken (string id, string userName) {
            var identity = GenerateClaimsIdentity (id, userName);

            var claims = new [] {
                new Claim (JwtRegisteredClaimNames.Sub, userName),
                new Claim (JwtRegisteredClaimNames.Jti, await _jwtIssuerOptions.JtiGenerator ()),
                new Claim (JwtRegisteredClaimNames.Iat, _jwtIssuerOptions.IssuedAt.ToUnixEpochDate ().ToString (), ClaimValueTypes.Integer64),
                identity.FindFirst (Constants.JwtClaimIdentifiers_Rol),
                identity.FindFirst (Constants.JwtClaimIdentifiers_Id)
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken (
                _jwtIssuerOptions.Issuer,
                _jwtIssuerOptions.Audience,
                claims,
                _jwtIssuerOptions.NotBefore,
                _jwtIssuerOptions.Expiration,
                _jwtIssuerOptions.SigningCredentials
            );

            return new AccessToken (
                _jwtTokenHandler.WriteToken (jwt),
                (int) _jwtIssuerOptions.ValidFor.TotalSeconds
            );
        }

        #endregion
    }
}