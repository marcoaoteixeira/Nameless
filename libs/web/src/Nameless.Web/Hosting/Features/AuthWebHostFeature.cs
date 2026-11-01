using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Auth;
using Nameless.Web.Hosting.Options;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers authentication and authorization, and uses their middlewares.
/// </summary>
public class AuthWebHostFeature : HostFeature<AuthFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        ConfigureAuthorization(builder.Services, options);
        ConfigureAuthentication(builder.Services, builder.Configuration, options);
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseAuthentication();
        app.UseAuthorization();
    }

    private static void ConfigureAuthorization(IServiceCollection services, AuthFeatureOptions options) {
        services.AddAuthorization(options.ConfigureAuthorization ?? (_ => { }));
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration, AuthFeatureOptions options) {
        var wrapper = new AuthenticationBuilderWrapper(services);
        var configure = options.ConfigureAuthentication ?? ConfigureDefaultAuthentication;

        configure(wrapper, configuration);

        wrapper.Apply();
    }

    private static void ConfigureDefaultAuthentication(IAuthenticationBuilder builder, IConfiguration? configuration) {
        builder.Configure(opts => opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opts => {
                   var section = configuration?
                                 .GetSectionFor<JsonWebTokenOptions>()
                                 .GetChildren()
                                 .FirstOrDefault();

                   if (section is null) { return; }

                   var jwt = section.GetOrThrow<JsonWebTokenOptions>();

                   opts.Authority = jwt.Authority;
                   opts.TokenValidationParameters = new TokenValidationParameters {
                       ValidIssuers = jwt.Issuers,
                       ValidateIssuer = jwt.ValidateIssuer,

                       ValidAudiences = jwt.Audiences,
                       ValidateAudience = jwt.ValidateAudience,

                       ValidateLifetime = jwt.ValidateLifetime,

                       ClockSkew = jwt.ClockSkew
                   };
               });
    }
}
