using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Attributes;
using Nameless.Auth.OAuth;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.ObjectModel;
using Scalar.AspNetCore;

namespace Nameless.Web.Scalar;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class WebApplicationExtensions {
    private const string LOG_TAG = "SCALAR";

    /// <param name="self">
    ///     The current instance of <see cref="WebApplication"/> class.
    /// </param>
    extension(WebApplication self) {
        public WebApplication UseScalar(Action<ScalarRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            // Do not expose Scalar on PROD environment
            if (self.Environment.IsProduction()) { return self; }

            var combination = Delegate.Combine(DefaultScalarConfiguration, settings.ConfigureScalar);

            self.MapScalarApiReference((Action<ScalarOptions, HttpContext>)combination);

            return self;

            void DefaultScalarConfiguration(ScalarOptions options, HttpContext context) {
                options
                    .WithTitle(self.Environment.ApplicationName)
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
                    .SortOperationsByMethod();

                if (settings.UseDefaultHttpAuthentication) {
                    options.AddHttpAuthentication(
                        securitySchemeName: JwtBearerDefaults.AuthenticationScheme,
                        configureScheme: scheme => ConfigureScalarHttpAuthentication(scheme, context)
                    );
                }
            }
        }
    }

    private static void ConfigureScalarHttpAuthentication(ScalarHttpSecurityScheme scheme, HttpContext context) {
        var logger = context.RequestServices.GetLogger<ScalarHttpSecurityScheme>();
        
        try {
            var provider = context.RequestServices.GetRequiredService<IOAuthAuthorizationTokenProvider>();
            var oauth = context.RequestServices.GetRequiredService<IConfiguration>().GetSection<OAuthOptions>();
            var request = CreateOAuthAuthorizationTokenRequest(oauth);
            var timeout = oauth.GetValue<int>(nameof(OAuthOptions.Timeout));
            var response = provider.GetToken(request, timeout >= 0 ? timeout : -1);

            response.Match(
                onSuccess: value => scheme.WithToken(value.AccessToken),
                onFailure: failure => {
                    scheme.WithToken(string.Empty);

                    CommonLog.Warning(
                        logger,
                        reason: failure.Flatten(),
                        tag: LOG_TAG
                    );
                }
            );
        }
        catch (Exception ex) {
            CommonLog.Failure(logger, ex, tag: LOG_TAG);

            throw;
        }
    }

    private static OAuthAuthorizationTokenRequest CreateOAuthAuthorizationTokenRequest(IConfigurationSection oauth) {
        var sectionName = ConfigurationSectionNameAttribute.GetSectionName<OAuthOptions>();

        return new OAuthAuthorizationTokenRequest {
            ClientId = oauth.GetValue<string>(nameof(OAuthAuthorizationTokenRequest.ClientId)) ??
                       throw new MissingConfigurationException(
                           section: sectionName,
                           key: nameof(OAuthAuthorizationTokenRequest.ClientId)
                       ),
            ClientSecret = oauth.GetValue<string>(nameof(OAuthAuthorizationTokenRequest.ClientSecret)) ??
                           throw new MissingConfigurationException(
                               section: sectionName,
                               key: nameof(OAuthAuthorizationTokenRequest.ClientSecret)
                           ),
            GrantType = oauth.GetValue<string>(nameof(OAuthAuthorizationTokenRequest.GrantType)),
            Audience = oauth.GetValue<string>(nameof(OAuthAuthorizationTokenRequest.Audience))
        };
    }
}
