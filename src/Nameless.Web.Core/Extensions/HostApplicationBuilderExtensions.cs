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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nameless.Web;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class HostApplicationBuilderExtensions {
    extension<THostApplicationBuilder>(THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {
        public THostApplicationBuilder RegisterAntiforgery(Action<AntiforgeryOptions>? configure = null) {
            self.Services
                .AddAntiforgery(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterOutputCache(Action<OutputCacheOptions>? configure = null) {
            self.Services
                .AddOutputCache(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterAuthorization(Action<AuthorizationOptions>? configure = null) {
            self.Services
                .AddAuthorization(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterAuthentication(Action<AuthenticationOptions>? configure = null) {
            self.Services
                .AddAuthentication(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterJwtBearerAuthentication(Action<JwtBearerOptions>? configure = null) {
            self.Services
                .AddAuthentication(opts => {
                    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterRateLimiter(Action<RateLimiterOptions>? configure = null) {
            self.Services
                .AddRateLimiter(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterCors(Action<CorsOptions>? configure = null) {
            self.Services
                .AddCors(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterDataProtection(Action<DataProtectionOptions>? configure = null) {
            self.Services
                .AddDataProtection(configure ?? (_ => { }));

            return self;
        }

        public THostApplicationBuilder RegisterRequestTimeouts(Action<RequestTimeoutOptions>? configure = null) {
            self.Services
                .AddRequestTimeouts(configure ?? (_ => { }));

            return self;
        }
    }
}