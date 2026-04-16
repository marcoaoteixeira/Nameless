using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nameless.Helpers;

namespace Nameless.Web.Auth;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterAuth(Action<AuthRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.AddAuthorization(settings.ConfigureAuthorization);

            if (!settings.UseJwtBearer) {
                self.AddAuthentication(settings.ConfigureAuthentication);

                return self;
            }

            var authentication = self.AddAuthentication(opts =>
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme
            );

            if (settings.UseJwtBearerDefaultConfiguration) {
                authentication.AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    opts => DefaultJwtBearerConfiguration(opts, configuration)
                );
            }

            foreach (var jwtBearer in settings.JwtBearerConfigurations) {
                authentication.AddJwtBearer(
                    jwtBearer.Key,
                    jwtBearer.Value
                );
            }

            return self;
        }
    }

    private static void DefaultJwtBearerConfiguration(JwtBearerOptions opts, IConfiguration? configuration) {
        var jwt = configuration?.GetOptions<JsonWebTokenOptions>()
            ?? throw new InvalidOperationException("JSON web token configuration is missing.");

        opts.Authority = jwt.Authority;
        opts.TokenValidationParameters = new TokenValidationParameters {
            ValidIssuers = jwt.Issuers,
            ValidateIssuer = jwt.ValidateIssuer,

            ValidAudiences = jwt.Audiences,
            ValidateAudience = jwt.ValidateAudience,

            ValidateLifetime = jwt.ValidateLifetime,

            ClockSkew = jwt.ClockSkew
        };
    }
}
