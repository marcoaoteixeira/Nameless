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

namespace Nameless.Web;

public static class WebApplicationBuilderExtensions {
    public static WebApplicationBuilder RegisterAntiforgery(this WebApplicationBuilder self, Action<AntiforgeryOptions>? configure = null) {
        self.Services
            .AddAntiforgery(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterOutputCache(this WebApplicationBuilder self, Action<OutputCacheOptions>? configure = null) {
        self.Services
            .AddOutputCache(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterAuthorization(this WebApplicationBuilder self, Action<AuthorizationOptions>? configure = null) {
        self.Services
            .AddAuthorization(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterAuthentication(this WebApplicationBuilder self, Action<AuthenticationOptions>? configure = null) {
        self.Services
            .AddAuthentication(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterJwtBearerAuthentication(this WebApplicationBuilder self, Action<JwtBearerOptions>? configure = null) {
        self.Services
            .AddAuthentication(opts => {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterRateLimiter(this WebApplicationBuilder self, Action<RateLimiterOptions>? configure = null) {
        self.Services
            .AddRateLimiter(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterCors(this WebApplicationBuilder self, Action<CorsOptions>? configure = null) {
        self.Services
            .AddCors(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterDataProtection(this WebApplicationBuilder self, Action<DataProtectionOptions>? configure = null) {
        self.Services
            .AddDataProtection(configure ?? (_ => { }));

        return self;
    }

    public static WebApplicationBuilder RegisterRequestTimeouts(this WebApplicationBuilder self, Action<RequestTimeoutOptions>? configure = null) {
        self.Services
            .AddRequestTimeouts(configure ?? (_ => { }));

        return self;
    }
}
