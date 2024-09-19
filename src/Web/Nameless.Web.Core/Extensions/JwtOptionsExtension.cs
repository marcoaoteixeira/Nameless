using Microsoft.IdentityModel.Tokens;
using Nameless.Web.Options;

namespace Nameless.Web;

public static class JwtOptionsExtension {
    public static TokenValidationParameters GetTokenValidationParameters(this JwtOptions self) {
        Prevent.Argument.Null(self);

        return new TokenValidationParameters {
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
}