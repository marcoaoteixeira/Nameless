using Microsoft.IdentityModel.Tokens;
using Nameless.WebApplication.Options;

namespace Nameless.WebApplication {

    public static class JsonWebTokenOptionsExtension {
        #region Public Static Methods

        public static TokenValidationParameters GetTokenValidationParameters(this JsonWebTokenOptions self) {
            Prevent.Null(self, nameof(self));

            return new() {
                ValidateIssuer = self.ValidateIssuer,
                ValidateAudience = self.ValidateAudience,
                ValidateLifetime = self.ValidateLifetime,
                ValidateIssuerSigningKey = self.ValidateIssuerSigningKey,
                ValidIssuer = self.Issuer,
                ValidAudience = self.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(self.Secret.GetBytes()),
                ClockSkew = TimeSpan.FromSeconds(self.MaxClockSkew)
            };
        }

        #endregion
    }
}
