using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nameless.Attributes;
using Nameless.Configuration;
using Nameless.Helpers;

namespace Nameless.Web.Auth;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterAuth(Action<AuthRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            return self.AddAuthorization(settings.ConfigureAuthorization ?? (_ => { }))
                       .AddAuthentication(settings, configuration);
        }

        private IServiceCollection AddAuthentication(AuthRegistration settings, IConfiguration? configuration) {
            var wrapper = new AuthenticationBuilderWrapper(self);
            var configure = settings.ConfigureAuthentication ?? (builder => ConfigureDefaultAuthentication(builder, configuration));

            configure(wrapper);

            wrapper.Apply();

            return self;
        }
    }

    private static void ConfigureDefaultAuthentication(IAuthenticationBuilder builder, IConfiguration? configuration) {
        builder.Configure(opts => opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opts => {
                   if (configuration is null) { return; }

                   const string SectionName = "Default";
                   var sectionPath = $"{ConfigurationSectionNameAttribute.GetSectionName<JsonWebTokenOptions>()}:{SectionName}";
                   var jwt = configuration.GetSection<JsonWebTokenOptions>()
                                          .GetOptions<JsonWebTokenOptions>(SectionName) ??
                             throw new MissingConfigurationException(section: sectionPath);

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