using Microsoft.AspNetCore.Authorization;

namespace Nameless.Web.Auth;

/// <summary>
///     Authorization/Authentication registration settings.
/// </summary>
public class AuthRegistration {
    /// <summary>
    ///     Gets or sets the action used to configure authorization
    ///     options for the application.
    /// </summary>
    public Action<AuthorizationOptions>? ConfigureAuthorization { get; set; }

    /// <summary>
    ///     Gets or sets the action used to configure authentication
    ///     options for the application.
    /// </summary>
    public Action<IAuthenticationBuilder>? ConfigureAuthentication { get; set; }
}