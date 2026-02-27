using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap.Infrastructure;
using Nameless.Mediator;
using Nameless.Microservices.Infrastructure.Auth;
using Nameless.Validation.FluentValidation;
using Nameless.Web.HealthChecks;
using Nameless.Web.MinimalEndpoints;
using Nameless.Web.Observability;
using Nameless.Web.OpenApi;
using Nameless.Web.RateLimiter;
using Nameless.Web.ServiceDiscovery;

namespace Nameless.Microservices.StartUp;

/// <summary>
///     Provides configuration settings for a web application, allowing
///     customization of various services and middleware components.
/// </summary>
/// <remarks>
///     The <see cref="WebAppSettings"/> class enables developers to enable
///     or disable specific features such as authentication, logging, health
///     checks, and more. It also provides delegates for configuring
///     additional services and middleware, offering flexibility in
///     application setup. Use this class to tailor the application's startup
///     behavior and service registration according to the needs of your
///     environment or deployment scenario.
///     Or just skip it and build your own way, that's fine too. :)
/// </remarks>
public class WebAppSettings {
    /// <summary>
    ///     Gets or sets the arguments.
    /// </summary>
    public string[] Args { get; set; } = [];

    /// <summary>
    ///     Gets or sets the array of assemblies to scan types.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    ///     Gets or sets a delegate for configure additional services.
    /// </summary>
    public Action<IHostApplicationBuilder> ConfigureAdditionalServices { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets a delegate for configure middlewares before Routing
    ///     services.
    /// </summary>
    public Action<IApplicationBuilder> UseBeforeRouting { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets a delegate for configure middlewares before startup.
    /// </summary>
    public Action<IApplicationBuilder> UseBeforeStartup { get; set; } = _ => { };

    /// <summary>
    ///     Whether it should disable antiforgery services.
    /// </summary>
    public bool DisableAntiforgery { get; set; }

    /// <summary>
    ///     Whether it should disable authentication/authorization services.
    /// </summary>
    public bool DisableAuth { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure authentication/authorization services.
    /// </summary>
    public Action<AuthRegistrationSettings>? ConfigureAuth { get; set; }

    /// <summary>
    ///     Whether it should disable bootstrap services.
    /// </summary>
    public bool DisableBootstrap { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure bootstrap services.
    /// </summary>
    public Action<BootstrapRegistrationSettings>? ConfigureBootstrap { get; set; }

    /// <summary>
    ///     Whether it should disable CORS services.
    /// </summary>
    public bool DisableCors { get; set; }

    /// <summary>
    ///     Whether it should disable data protection services.
    /// </summary>
    public bool DisableDataProtection { get; set; }

    /// <summary>
    ///     Whether it should disable exception handling services.
    /// </summary>
    public bool DisableExceptionHandling { get; set; }

    /// <summary>
    ///     Whether it should disable health check services.
    /// </summary>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure health check services.
    /// </summary>
    public Action<HealthChecksRegistrationSettings>? ConfigureHealthChecks { get; set; }

    /// <summary>
    ///     Whether it should disable request correlation ID services.
    /// </summary>
    public bool DisableHttpContextCorrelation { get; set; }

    /// <summary>
    ///     Whether it should disable HTTPS redirection services.
    /// </summary>
    public bool DisableHttpsRedirection { get; set; }

    /// <summary>
    ///     Whether it should disable logging services.
    /// </summary>
    public bool DisableLogging { get; set; }

    /// <summary>
    ///     Whether it should disable mediator services.
    /// </summary>
    public bool DisableMediator { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure mediator services.
    /// </summary>
    public Action<MediatorRegistrationSettings>? ConfigureMediator { get; set; }

    /// <summary>
    ///     Whether it should disable minimal endpoints services.
    /// </summary>
    public bool DisableMinimalEndpoints { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure minimal endpoints services.
    /// </summary>
    public Action<MinimalEndpointsRegistrationSettings>? ConfigureMinimalEndpoints { get; set; }

    /// <summary>
    ///     Whether it should disable OpenAPI services.
    /// </summary>
    public bool DisableOpenApi { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure OpenAPI services.
    /// </summary>
    public Action<OpenApiRegistrationSettings>? ConfigureOpenApi { get; set; }

    /// <summary>
    ///     Whether it should disable OpenTelemetry services.
    /// </summary>
    public bool DisableOpenTelemetry { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure OpenTelemetry services.
    /// </summary>
    public Action<OpenTelemetryRegistrationSettings>? ConfigureOpenTelemetry { get; set; }

    /// <summary>
    ///     Whether it should disable output cache services.
    /// </summary>
    public bool DisableOutputCache { get; set; }

    /// <summary>
    ///     Whether it should disable request rate limiter services.
    /// </summary>
    public bool DisableRateLimiter { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure request rate limiter services.
    /// </summary>
    public Action<RateLimiterRegistrationSettings>? ConfigureRateLimiter { get; set; }

    /// <summary>
    ///     Whether it should disable request timeouts services.
    /// </summary>
    public bool DisableRequestTimeouts { get; set; }

    /// <summary>
    ///     Whether it should disable routing services.
    /// </summary>
    public bool DisableRouting { get; set; }

    /// <summary>
    ///     Whether it should disable service discovery (ASPIRE).
    /// </summary>
    public bool DisableServiceDiscovery { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure service discover.
    /// </summary>
    public Action<ServiceDiscoveryRegistrationSettings>? ConfigureServiceDiscovery { get; set; }

    /// <summary>
    ///     Whether it should disable validation services.
    /// </summary>
    public bool DisableValidation { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure validation services.
    /// </summary>
    public Action<ValidationRegistrationSettings>? ConfigureValidation { get; set; }
}
