using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Nameless.Web.Auth;

/// <summary>
///     Servers as a wrapper for <see cref="AuthenticationBuilder"/>.
/// </summary>
public interface IAuthenticationBuilder {
    /// <summary>
    ///     Configures the authentication service.
    /// </summary>
    /// <param name="configure">
    ///     A delegate to configure the authentication options.
    /// </param>
    /// <returns>
    ///     The current <see cref="IAuthenticationBuilder"/> instance so
    ///     other actions can be chained.
    /// </returns>
    IAuthenticationBuilder Configure(Action<AuthenticationOptions> configure);

    /// <summary>
    ///     Adds a <see cref="AuthenticationScheme"/> which can be used
    ///     by <see cref="IAuthenticationService"/>.
    /// </summary>
    /// <typeparam name="TOptions">
    ///     The <see cref="AuthenticationSchemeOptions"/> type to configure the handler.
    /// </typeparam>
    /// <typeparam name="THandler">
    ///     The <see cref="AuthenticationHandler{TOptions}"/> used to handle this scheme.
    /// </typeparam>
    /// <param name="configure">
    ///     Used to configure the scheme options.
    /// </param>
    /// <param name="authenticationScheme">
    ///     The name of this scheme. If no value is provided, default scheme
    ///     is <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </param>
    /// <param name="displayName">
    ///     The display name of this scheme.
    /// </param>
    /// <returns>
    ///     The current <see cref="IAuthenticationBuilder"/> instance so
    ///     other actions can be chained.
    /// </returns>
    IAuthenticationBuilder AddScheme<TOptions, THandler>(Action<TOptions>? configure, string? authenticationScheme = null, string? displayName = null)
        where TOptions : AuthenticationSchemeOptions, new()
        where THandler : AuthenticationHandler<TOptions>;

    /// <summary>
    ///     Adds a <see cref="PolicySchemeHandler"/> based authentication
    ///     handler which can be used to redirect to other authentication
    ///     schemes.
    /// </summary>
    /// <param name="configure">
    ///     Used to configure the scheme options.
    /// </param>
    /// <param name="authenticationScheme">
    ///     The name of this scheme. If no value is provided, default scheme
    ///     is <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </param>
    /// <param name="displayName">
    ///     The display name of this scheme.
    /// </param>
    /// <returns>
    ///     The current <see cref="IAuthenticationBuilder"/> instance so
    ///     other actions can be chained.
    /// </returns>
    IAuthenticationBuilder AddPolicyScheme(Action<PolicySchemeOptions> configure, string? authenticationScheme = null, string? displayName = null);

    /// <summary>
    ///     Enables JWT-bearer authentication using the specified scheme.
    /// <para>
    ///     JWT bearer authentication performs authentication by extracting
    ///     and validating a JWT token from the <c>Authorization</c> request
    ///     header.
    /// </para>
    /// </summary>
    /// <param name="configure">
    ///     A delegate that allows configuring <see cref="JwtBearerOptions"/>.
    /// </param>
    /// <param name="authenticationScheme">
    ///     The authentication scheme. If no value is provided, default scheme
    ///     is <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </param>
    /// <param name="displayName">
    ///     The display name for the authentication handler.
    /// </param>
    /// <returns>
    ///     The current <see cref="IAuthenticationBuilder"/> instance so
    ///     other actions can be chained.
    /// </returns>
    IAuthenticationBuilder AddJwtBearer(Action<JwtBearerOptions> configure, string? authenticationScheme = null, string? displayName = null);
}