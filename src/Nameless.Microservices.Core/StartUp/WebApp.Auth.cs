using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nameless.Helpers;
using Nameless.Microservices.Infrastructure.Auth;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder AuthConfig(WebAppSettings settings) {
            if (settings.DisableAuth) { return self; }

            var config = ActionHelper.FromDelegate(settings.ConfigureAuth);

            var auth = self.Services.AddAuthorization(config.ConfigureAuthorization);

            if (!config.UseJwtBearer) {
                auth.AddAuthentication(config.ConfigureAuthentication);

                return self;
            }

            var builder = auth.AddAuthentication(opts =>
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme
            );

            if (config.JwtBearerConfigurations.Count == 0) {
                config.RegisterJwtBearerConfiguration(DefaultJwtBearerConfiguration);
            }

            foreach (var jwtBearer in config.JwtBearerConfigurations) {
                builder.AddJwtBearer(jwtBearer.Key, jwtBearer.Value);
            }

            return self;

            void DefaultJwtBearerConfiguration(JwtBearerOptions opts) {
                var jwtOpts = self.Configuration.GetOptions<JsonWebTokenOptions>();

                opts.Authority = jwtOpts.Authority;
                opts.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuers = jwtOpts.Issuers,
                    ValidateIssuer = jwtOpts.ValidateIssuer,

                    ValidAudiences = jwtOpts.Audiences,
                    ValidateAudience = jwtOpts.ValidateAudience,

                    ValidateLifetime = jwtOpts.ValidateLifetime,

                    ClockSkew = jwtOpts.ClockSkew
                };
            }
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the authentication/authorization service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseAuth(WebAppSettings settings) {
            if (settings.DisableAuth) { return self; }

            self.UseAuthentication()
                .UseAuthorization();

            return self;
        }
    }
}
