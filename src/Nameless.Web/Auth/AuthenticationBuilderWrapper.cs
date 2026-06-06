using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Auth;

public class AuthenticationBuilderWrapper : IAuthenticationBuilder {
    private readonly IServiceCollection _services;

    private Action<AuthenticationBuilder>? Compose { get; set; }
    private Action<AuthenticationOptions>? Options { get; set; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="AuthenticationBuilderWrapper"/> class.
    /// </summary>
    /// <param name="services">
    ///     The current instance of <see cref="IServiceCollection"/>.
    /// </param>
    public AuthenticationBuilderWrapper(IServiceCollection services) {
        _services = services;
    }

    public IAuthenticationBuilder Configure(Action<AuthenticationOptions> configure) {
        Options = configure;

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddScheme<TOptions, THandler>(Action<TOptions>? configure, string? authenticationScheme = null, string? displayName = null)
        where TOptions : AuthenticationSchemeOptions, new()
        where THandler : AuthenticationHandler<TOptions> {
        Compose += builder => {
            builder.AddScheme<TOptions, THandler>(
                authenticationScheme: authenticationScheme ?? JwtBearerDefaults.AuthenticationScheme,
                displayName,
                configureOptions: configure
            );
        };

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddPolicyScheme(Action<PolicySchemeOptions> configure, string? authenticationScheme = null, string? displayName = null) {
        Compose += builder => {
            builder.AddPolicyScheme(
                authenticationScheme: authenticationScheme ?? JwtBearerDefaults.AuthenticationScheme,
                displayName,
                configureOptions: configure
            );
        };

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddJwtBearer(Action<JwtBearerOptions> configure, string? authenticationScheme = null, string? displayName = null) {
        Compose += builder => {
            builder.AddJwtBearer(
                authenticationScheme: authenticationScheme ?? JwtBearerDefaults.AuthenticationScheme,
                displayName,
                configureOptions: configure
            );
        };

        return this;
    }

    /// <summary>
    ///     Applies all configuration.
    /// </summary>
    public void Apply() {
        var authentication = _services.AddAuthentication(Options ?? (_ => { }));

        Compose?.Invoke(authentication);
        Compose = null;
    }
}