using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Nameless.Microservices.Infrastructure.Auth;

/// <summary>
///     Authorization/Authentication registration settings.
/// </summary>
public class AuthRegistrationSettings {
    private readonly Dictionary<string, Action<JwtBearerOptions>> _jwtBearerConfigurations = [];

    /// <summary>
    ///     Gets or sets the action used to configure authorization
    ///     options for the application.
    /// </summary>
    public Action<AuthorizationOptions> ConfigureAuthorization { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets the action used to configure authentication
    ///     options for the application.
    /// </summary>
    public Action<AuthenticationOptions> ConfigureAuthentication { get; set; } = _ => { };

    /// <summary>
    ///     Whether it should use JWT Bearer authentication.
    /// </summary>
    /// <remarks>
    ///     When set to <see langword="true"/>, the application will be
    ///     configured to use JWT Bearer tokens for authentication.
    /// </remarks>
    public bool UseJwtBearer { get; set; } = true;

    /// <summary>
    ///     Gets the collection of JWT Bearer authentication scheme configurations,
    ///     allowing customization of authentication options for multiple schemes.
    /// </summary>
    public IReadOnlyDictionary<string, Action<JwtBearerOptions>> JwtBearerConfigurations => _jwtBearerConfigurations;

    /// <summary>
    ///     Registers a JWT Bearer authentication configuration using the
    ///     specified options and authentication scheme.
    /// </summary>
    /// <param name="configure">
    ///     An action that configures the <see cref="JwtBearerOptions"/>
    ///     for the authentication scheme. This delegate is used to customize
    ///     the behavior of JWT Bearer authentication.
    /// </param>
    /// <param name="scheme">
    ///     The authentication scheme name to associate with the configuration.
    ///     If <see langword="null"/>, the default scheme is used.
    /// </param>
    /// <returns>
    ///     The current <see cref="AuthRegistrationSettings"/> instance, so
    ///     other actions can be chained.
    /// </returns>
    public AuthRegistrationSettings RegisterJwtBearerConfiguration(Action<JwtBearerOptions> configure, string? scheme = null) {
        _jwtBearerConfigurations[
            scheme ?? JwtBearerDefaults.AuthenticationScheme
        ] = configure;

        return this;
    }
}