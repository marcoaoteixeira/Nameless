using Microsoft.AspNetCore.Builder;
using Nameless.Helpers;

namespace Nameless.Microservices.StartUp;

/// <summary>
///     Provides a framework for configuring, building, and running a web
///     application with customizable settings and middleware components.
/// </summary>
/// <remarks>
///     The <see cref="WebApp"/> class encapsulates the setup and execution
///     of a web application using a builder pattern. It supports extensive
///     configuration through the <see cref="WebAppSettings"/> parameter.
///     This class is intended to simplify the initialization and management
///     of web applications by centralizing configuration and startup logic.
/// </remarks>
public sealed class WebApp {
    private readonly Lazy<WebApplicationBuilder> _builder;
    private readonly Lazy<WebApplication> _app;

    private WebAppSettings Settings { get; }

    private WebApplicationBuilder Builder => _builder.Value;

    private WebApplication App => _app.Value;

    private WebApp(WebAppSettings settings) {
        _builder = new Lazy<WebApplicationBuilder>(CreateWebApplicationBuilder);
        _app = new Lazy<WebApplication>(CreateWebApplication);

        Settings = settings;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="WebApp"/> class,
    ///     optionally applying custom configuration settings.
    /// </summary>
    /// <param name="settings">
    ///     An optional delegate that configures the
    ///     <see cref="WebAppSettings"/> for the new <see cref="WebApp"/>
    ///     instance. If <see langword="null"/>, default settings are used.
    /// </param>
    /// <returns>
    ///     A new <see cref="WebApp"/> instance configured with the specified
    ///     settings or default settings if none are provided.
    /// </returns>
    public static WebApp Create(Action<WebAppSettings>? settings = null) {
        return new WebApp(
            ActionHelper.FromDelegate(settings)
        );
    }

    /// <summary>
    ///     Initiates the synchronous execution of the application.
    /// </summary>
    public void Run() {
        App.Run();
    }

    /// <summary>
    ///     Initiates the asynchronous execution of the application.
    /// </summary>
    /// <returns>
    ///     A task representing the asynchronous run operation of the
    ///     application. The task completes when the application has
    ///     finished running.
    /// </returns>
    public Task RunAsync() {
        return App.RunAsync();
    }

    private WebApplicationBuilder CreateWebApplicationBuilder() {
        return WebApplication
            .CreateBuilder(Settings.Args)
            .AntiforgeryConfig(Settings)
            .AuthConfig(Settings)
            .BootstrapConfig(Settings)
            .CommonConfig()
            .CorsConfig(Settings)
            .DataProtectionConfig(Settings)
            .ExceptionHandlingConfig(Settings)
            .HealthChecksConfig(Settings)
            .HttpContextCorrelationConfig(Settings)
            .LoggingConfig(Settings)
            .MediatorConfig(Settings)
            .MinimalEndpointsConfig(Settings)
            .OpenApiConfig(Settings)
            .OpenTelemetryConfig(Settings)
            .OutputCacheConfig(Settings)
            .RateLimiterConfig(Settings)
            .OutputCacheConfig(Settings)
            .RequestTimeoutsConfig(Settings)
            .ServiceDiscoveryConfig(Settings)
            .ValidationConfig(Settings)
            // Custom config should be the last one since it can
            // replace previous registered services.
            .CustomConfig(Settings.ConfigureAdditionalServices);
    }

    private WebApplication CreateWebApplication() {
        // Here we are configuring the application middlewares
        // It's important that they respect a specific order
        // of registration to work properly.
        // Check ASP.NET Core documentation for more information.
        return Builder
            .Build()
            .UseExceptionHandling(Settings)
            .UseLogging(Settings)
            .UseHttpsRedirection(Settings)
            .UseHttpContextCorrelation(Settings)

            // Important: UseRouting must be called here before
            // "UseEndpoints" or "UseAuthorization".
            .UseRouting(Settings)

            .UseCors(Settings)
            .UseRateLimiter(Settings)
            .UseOutputCache(Settings)
            .UseRequestTimeouts(Settings)
            .UseAuth(Settings)
            .UseAntiforgery(Settings)
            .UseBootstrap(Settings)
            .UseOpenApi(Settings)
            .UseMinimalEndpoints(Settings)
            .UseHealthChecks(Settings)

            .UseCustom(Settings.UseBeforeStartup);
    }
}