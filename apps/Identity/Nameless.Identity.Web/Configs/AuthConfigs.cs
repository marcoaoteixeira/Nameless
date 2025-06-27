using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Nameless.Identity.Web.Configs;

public static class AuthConfigs {
    public static WebApplicationBuilder ConfigureAuthServices(this WebApplicationBuilder self, Action<AuthOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new AuthOptions();

        innerConfigure(options);

        self.Services
            .AddAuthorization(options.ConfigureAuthorization)
            .AddAuthentication(opts => {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options.ConfigureJwtBearer);

        return self;
    }
}

public sealed record AuthOptions {
    public Action<AuthorizationOptions> ConfigureAuthorization { get; init; } = _ => { };
    public Action<JwtBearerOptions> ConfigureJwtBearer { get; init; } = _ => { };
}