using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Auth;
using Nameless.Auth.OAuth;
using Nameless.ObjectModel;
using Scalar.AspNetCore;

namespace Nameless.Web.Scalar.Auth.OAuth;

/// <summary>
///     Simple helper to set HTTP security with OAuth access token
///     "auto" provider.
/// </summary>
public static class OAuthHttpAuthenticationHelper {
    private const string TAG = "SCALAR_HTTP_SECURITY";
    private const string OAUTH_CLIENT_CREDENTIALS_CONFIG_SECTION = "OAuthClientCredentials";

    /// <summary>
    ///     Configures Scalar HTTP security to use OAuth
    ///     authentication token provider.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="ScalarOptions"/> instance.
    /// </param>
    /// <param name="context">
    ///     The HTTP context.
    /// </param>
    public static void SetOAuthHttpSecurity(this ScalarOptions self, HttpContext context) {
        self.AddHttpAuthentication(
            securitySchemeName: JwtBearerDefaults.AuthenticationScheme,
            configureScheme: scheme => GetJwtBearerToken(scheme, context)
        );
    }

    private static void GetJwtBearerToken(ScalarHttpSecurityScheme scheme, HttpContext context) {
        var logger = context.RequestServices.GetLogger(typeof(OAuthHttpAuthenticationHelper));
        var provider = context.RequestServices.GetService<IAccessTokenProvider<OAuthTokenRequest>>();
        if (provider is null) {
            CommonLog.Debug(
                logger,
                message: "OAuth access token provider unavailable.",
                tag: TAG
            );

            return;
        }

        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var oauthClientCredentials = configuration.GetSection(OAUTH_CLIENT_CREDENTIALS_CONFIG_SECTION);
        if (!oauthClientCredentials.Exists()) {
            CommonLog.Warning(
                logger,
                message: $"Missing OAuth Client Credentials configuration section ({OAUTH_CLIENT_CREDENTIALS_CONFIG_SECTION}).",
                tag: TAG
            );

            return;
        }

        var request = new ClientCredentialsRequest {
            Scope = oauthClientCredentials.GetValue<string>(nameof(ClientCredentialsRequest.Scope)),
            Audience = oauthClientCredentials.GetValue<string>(nameof(ClientCredentialsRequest.Audience))
        };
        var response = provider.GetToken(request);

        response.Match(
            onSuccess: value => scheme.Token = value.Value,
            onFailure: failure => {
                scheme.Token = null;

                Log.UnableRetrieveAccessToken(
                    logger,
                    reason: failure.Flatten(),
                    tag: TAG
                );
            }
        );
    }
}
