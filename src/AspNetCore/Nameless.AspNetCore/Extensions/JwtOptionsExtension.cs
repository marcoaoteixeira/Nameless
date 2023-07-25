using Microsoft.IdentityModel.Tokens;
using Nameless.AspNetCore.Options;

namespace Nameless.AspNetCore {
    public static class JwtOptionsExtension {
        #region Public Static Methods

        public static TokenValidationParameters GetTokenValidationParameters(this JwtOptions self)
            => new() {
                ValidateIssuer = self.ValidateIssuer,
                ValidateAudience = self.ValidateAudience,
                ValidateLifetime = self.ValidateLifetime,
                ValidateIssuerSigningKey = self.ValidateIssuerSigningKey,
                ValidIssuer = self.Issuer,
                ValidAudience = self.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(self.Secret.GetBytes()),
                ClockSkew = TimeSpan.FromSeconds(self.MaxClockSkew)
            };

        #endregion
    }
}
