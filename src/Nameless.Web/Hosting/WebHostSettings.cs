using System.Reflection;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Bootstrap;
using Nameless.Logging.Serilog;
using Nameless.Mediator;
using Nameless.Validation.FluentValidation;
using Nameless.Web.Auth;
using Nameless.Web.ErrorHandling;
using Nameless.Web.HealthCheck;
using Nameless.Web.Observability;
using Nameless.Web.OpenApi;
using Nameless.Web.RateLimiter;
using Nameless.Web.Scalar;
using Nameless.Web.ServiceDiscovery;
using Nameless.Workers;

namespace Nameless.Web.Hosting;

/// <summary>
///     Provides configuration settings for a web application, allowing
///     customization of various services and middleware components.
/// </summary>
/// <remarks>
///     The <see cref="WebHostSettings"/> class enables developers to enable
///     or disable specific features such as authentication, logging, health
///     checks, and more. It also provides delegates for configuring
///     additional services and middleware, offering flexibility in
///     application setup. Use this class to tailor the application's startup
///     behavior and service registration according to the needs of your
///     environment or deployment scenario.
///     Or just skip it and build your own way, that's fine too. :)
/// </remarks>
public class WebHostSettings {
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
    public Action<IServiceCollection, IConfiguration, IHostEnvironment>? ConfigureAdditionalServices { get; set; }

    /// <summary>
    ///     Gets or sets a delegate that executes before the <c>UseRouting</c>
    ///     middleware configuration.
    /// </summary>
    public Action<IApplicationBuilder> ExecuteBeforeRouting { get; set; } = _ => { };

    /// <summary>
    ///     Gets or sets a delegate that executes before the application startup.
    /// </summary>
    public Action<IApplicationBuilder> ExecuteBeforeStartup { get; set; } = _ => { };

    /// <summary>
    ///     Whether it should disable antiforgery services.
    /// </summary>
    public bool DisableAntiforgery { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to configure antiforgery.
    /// </summary>
    public Action<AntiforgeryOptions>? ConfigureAntiforgery { get; set; }

    /// <summary>
    ///     Whether it should disable authentication/authorization services.
    /// </summary>
    public bool DisableAuth { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure authentication/authorization.
    /// </summary>
    public Action<AuthRegistration>? ConfigureAuth { get; set; }

    /// <summary>
    ///     Whether it should disable bootstrap services.
    /// </summary>
    public bool DisableBootstrap { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to configure bootstrap.
    /// </summary>
    public Action<BootstrapRegistration>? ConfigureBootstrap { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to configure bootstrap warmup.
    /// </summary>
    public Action<BootstrapOptions>? ConfigureBootstrapWarmup { get; set; }

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
    ///     Gets or sets a delegate to configure the exception handling.
    /// </summary>
    public Action<ErrorHandlingRegistration>? ConfigureExceptionHandling { get; set; }
    
    /// <summary>
    ///     Whether it should disable health check services.
    /// </summary>
    public bool DisableHealthChecks { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure health check services.
    /// </summary>
    public Action<HealthCheckRegistration>? ConfigureHealthCheck { get; set; }

    /// <summary>
    ///     Whether it should disable request correlation ID services.
    /// </summary>
    public bool DisableHttpRequestCorrelation { get; set; }

    /// <summary>
    ///     Whether it should disable HTTPS redirection services.
    /// </summary>
    public bool DisableHttpsRedirection { get; set; }

    /// <summary>
    ///     Whether it should disable logging services.
    /// </summary>
    public bool DisableLogging { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to configure Serilog.
    /// </summary>
    public Action<SerilogRegistration>? ConfigureSerilog { get; set; }

    /// <summary>
    ///     Whether it should disable mediator services.
    /// </summary>
    public bool DisableMediator { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure mediator services.
    /// </summary>
    public Action<MediatorRegistration>? ConfigureMediator { get; set; }

    /// <summary>
    ///     Whether it should disable OpenAPI services.
    /// </summary>
    public bool DisableOpenApi { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure OpenAPI services.
    /// </summary>
    public Action<OpenApiRegistration>? ConfigureOpenApi { get; set; }

    /// <summary>
    ///     Whether it should disable Scalar feature.
    /// </summary>
    public bool DisableScalar { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure Scalar.
    /// </summary>
    public Action<ScalarRegistration>? ConfigureScalar { get; set; }

    /// <summary>
    ///     Whether it should disable OpenTelemetry services.
    /// </summary>
    public bool DisableOpenTelemetry { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure OpenTelemetry services.
    /// </summary>
    public Action<OpenTelemetryRegistration>? ConfigureOpenTelemetry { get; set; }

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
    public Action<RateLimiterRegistration>? ConfigureRateLimiter { get; set; }

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
    public Action<ServiceDiscoveryRegistration>? ConfigureServiceDiscovery { get; set; }

    /// <summary>
    ///     Whether it should disable validation services.
    /// </summary>
    public bool DisableValidator { get; set; }

    /// <summary>
    ///     Gets or sets a delegate for configure validation services.
    /// </summary>
    public Action<ValidatorRegistration>? ConfigureValidation { get; set; }

    /// <summary>
    ///     Whether it should disable Periodic Workers feature.
    /// </summary>
    public bool DisablePeriodicWorkers { get; set; }

    /// <summary>
    ///     Gets or sets a delegate to configure Periodic Workers feature.
    /// </summary>
    public Action<PeriodicWorkersRegistration>? ConfigurePeriodicWorkers { get; set; }

    /// <summary>
    ///     Whether it should disable Resilience feature.
    /// </summary>
    public bool DisableResilience { get; set; }

    /// <summary>
    ///     Whether it should disable Status Reporting feature.
    /// </summary>
    public bool DisableStatusReporting { get; set; }
}
