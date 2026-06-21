using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Auth;

public class AuthenticationBuilderWrapper : IAuthenticationBuilder {
    private readonly IServiceCollection _services;

    private Action<AuthenticationBuilder>? ConfigureBuilder { get; set; }
    private Action<AuthenticationOptions>? ConfigureOptions { get; set; }

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
        ConfigureOptions = configure;

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddScheme<TOptions, THandler>(string authenticationScheme, Action<TOptions>? configureOptions, string? displayName = null)
        where TOptions : AuthenticationSchemeOptions, new()
        where THandler : AuthenticationHandler<TOptions> {
        ConfigureBuilder += builder => builder.AddScheme<TOptions, THandler>(
            authenticationScheme,
            displayName,
            configureOptions
        );

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddPolicyScheme(string authenticationScheme, Action<PolicySchemeOptions> configureOptions, string? displayName = null) {
        ConfigureBuilder += builder => builder.AddPolicyScheme(
            authenticationScheme,
            displayName,
            configureOptions
        );

        return this;
    }

    /// <inheritdoc />
    public IAuthenticationBuilder AddJwtBearer(Action<JwtBearerOptions> configureOptions, string? authenticationScheme = null, string? displayName = null) {
        ConfigureBuilder += builder => builder.AddJwtBearer(
            authenticationScheme ?? JwtBearerDefaults.AuthenticationScheme,
            displayName,
            configureOptions
        );

        return this;
    }

    /// <summary>
    ///     Applies all configuration.
    /// </summary>
    public void Apply() {
        var authentication = _services.AddAuthentication(ConfigureOptions ?? (_ => { }));

        ConfigureBuilder?.Invoke(authentication);

        ConfigureBuilder = null;
        ConfigureOptions = null;
    }
}