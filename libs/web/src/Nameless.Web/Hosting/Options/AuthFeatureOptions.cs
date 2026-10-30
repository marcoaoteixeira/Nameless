using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Nameless.Web.Auth;

namespace Nameless.Web.Hosting.Options;

/// <summary>
///     Authorization/Authentication options.
/// </summary>
public class AuthFeatureOptions {
    /// <summary>
    ///     Gets or sets the action used to configure authorization
    ///     options for the application.
    /// </summary>
    public Action<AuthorizationOptions>? ConfigureAuthorization { get; set; }

    /// <summary>
    ///     Gets or sets the action used to configure authentication
    ///     options for the application.
    /// </summary>
    public Action<IAuthenticationBuilder, IConfiguration>? ConfigureAuthentication { get; set; }
}