using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Nameless.Web.IdentityModel.Jwt;

namespace Nameless.Web;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    public static THostApplicationBuilder RegisterAntiforgery<THostApplicationBuilder>(this THostApplicationBuilder self, Action<AntiforgeryOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddAntiforgery(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterOutputCache<THostApplicationBuilder>(this THostApplicationBuilder self, Action<OutputCacheOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddOutputCache(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterAuthorization<THostApplicationBuilder>(this THostApplicationBuilder self, Action<AuthorizationOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddAuthorization(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterAuthentication<THostApplicationBuilder>(this THostApplicationBuilder self, Action<AuthenticationOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddAuthentication(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterJwtBearerAuthentication<THostApplicationBuilder>(this THostApplicationBuilder self, Action<JwtBearerOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddAuthentication(opts => {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configure ?? DefaultConfigureJwtBearer);

        return self;

        void DefaultConfigureJwtBearer(JwtBearerOptions options) {
            var jsonWebTokenOptions = self.Configuration
                                          .GetSection(nameof(JsonWebTokenOptions))
                                          .Get<JsonWebTokenOptions>() ?? new JsonWebTokenOptions();

            if (string.IsNullOrWhiteSpace(jsonWebTokenOptions.Secret)) {
                throw new MissingSecretConfigurationException();
            }

            var securityKey = new SymmetricSecurityKey(jsonWebTokenOptions.Secret.GetBytes());

            options.TokenValidationParameters = new TokenValidationParameters {
                ValidAudiences = jsonWebTokenOptions.Audiences,
                ValidateAudience = jsonWebTokenOptions.ValidateAudience,
                ValidIssuer = jsonWebTokenOptions.Issuer,
                ValidateIssuer = jsonWebTokenOptions.ValidateIssuer,
                ValidateIssuerSigningKey = jsonWebTokenOptions.ValidateIssuerSigninKey,
                IssuerSigningKey = securityKey,
                ValidAlgorithms = [jsonWebTokenOptions.SecurityAlgorithm ?? SecurityAlgorithms.HmacSha256],
                ValidateLifetime = jsonWebTokenOptions.ValidateLifetime,
                ClockSkew = jsonWebTokenOptions.ClockSkew,
            };
        }
    }

    public static THostApplicationBuilder RegisterRateLimiter<THostApplicationBuilder>(this THostApplicationBuilder self, Action<RateLimiterOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddRateLimiter(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterCors<THostApplicationBuilder>(this THostApplicationBuilder self, Action<CorsOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddCors(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterDataProtection<THostApplicationBuilder>(this THostApplicationBuilder self, Action<DataProtectionOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddDataProtection(configure ?? (_ => { }));

        return self;
    }

    public static THostApplicationBuilder RegisterRequestTimeouts<THostApplicationBuilder>(this THostApplicationBuilder self, Action<RequestTimeoutOptions>? configure = null)
        where THostApplicationBuilder : IHostApplicationBuilder {
        self.Services
            .AddRequestTimeouts(configure ?? (_ => { }));

        return self;
    }
}
