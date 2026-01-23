using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Nameless.Web.Identity.Jwt;

namespace Nameless.Microservices.App.Configs;

public static class AuthConfig {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder ConfigureAuth() {
            // Configures the authorization and authentication services for the application.
            // This template uses JWT for authentication.

            self.Services
                .AddAuthorization()
                .AddAuthentication(opts => opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var jsonWebTokenOptions = self.Configuration
                                                  .GetSection(nameof(JsonWebTokenOptions))
                                                  .Get<JsonWebTokenOptions>() ?? new JsonWebTokenOptions();

                    var key = Defaults.Encoding.GetBytes(jsonWebTokenOptions.Secret ?? string.Empty);
                    var securityKey = new SymmetricSecurityKey(key);

                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidIssuer = jsonWebTokenOptions.Issuer,
                        ValidateIssuer = jsonWebTokenOptions.ValidateIssuer,
                        IssuerSigningKey = securityKey,
                        ValidateIssuerSigningKey = jsonWebTokenOptions.ValidateIssuerSigninKey,

                        ValidAudiences = jsonWebTokenOptions.Audiences,
                        ValidateAudience = jsonWebTokenOptions.ValidateAudience,

                        ValidateLifetime = jsonWebTokenOptions.ValidateLifetime,

                        ClockSkew = jsonWebTokenOptions.ClockSkew,
                    };
                });

            return self;
        }
    }
}
