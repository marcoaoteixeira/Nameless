using Microsoft.AspNetCore.Builder;
using Nameless.Helpers;
using Nameless.Web.Hosting.Configs;

namespace Nameless.Web.Hosting;

/// <summary>
///     Provides a framework for configuring, building, and running a web
///     application with customizable settings and middleware components.
/// </summary>
/// <remarks>
///     The <see cref="WebHostFactory"/> class encapsulates the setup and execution
///     of a web application using a builder pattern. It supports extensive
///     configuration through the <see cref="WebHostSettings"/> parameter.
///     This class is intended to simplify the initialization and management
///     of web applications by centralizing configuration and startup logic.
/// </remarks>
public sealed class WebHostFactory {
    private readonly Lazy<WebApplicationBuilder> _builder;
    private readonly Lazy<WebApplication> _app;

    private WebHostSettings Settings { get; }

    private WebApplicationBuilder Builder => _builder.Value;

    private WebApplication App => _app.Value;

    private WebHostFactory(WebHostSettings settings) {
        _builder = new Lazy<WebApplicationBuilder>(CreateWebApplicationBuilder);
        _app = new Lazy<WebApplication>(CreateWebApplication);

        Settings = settings;
    }

    /// <summary>
    ///     Creates a new instance of the <see cref="WebHostFactory"/> class,
    ///     optionally applying custom configuration settings.
    /// </summary>
    /// <param name="settings">
    ///     An optional delegate that configures the
    ///     <see cref="WebHostSettings"/> for the new <see cref="WebHostFactory"/>
    ///     instance. If <see langword="null"/>, default settings are used.
    /// </param>
    /// <returns>
    ///     A new <see cref="WebHostFactory"/> instance configured with the specified
    ///     settings or default settings if none are provided.
    /// </returns>
    public static WebHostFactory Create(Action<WebHostSettings>? settings = null) {
        return new WebHostFactory(
            ActionHelper.FromDelegate(settings)
        );
    }

    /// <summary>
    ///     Initiates the synchronous execution of the application.
    /// </summary>
    public void Run() {
        App.Warmup(Settings);
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
    public async Task RunAsync() {
        await App.WarmupAsync(Settings);
        await App.RunAsync();
    }

    private WebApplicationBuilder CreateWebApplicationBuilder() {
        return WebApplication
            .CreateBuilder(Settings.Args)

            .ConfigureAntiforgery(Settings)
            .ConfigureAuth(Settings)
            .ConfigureBootstrap(Settings)
            .ConfigureCommon()
            .ConfigureCors(Settings)
            .ConfigureDataProtection(Settings)
            .ConfigureExceptionHandling(Settings)
            .ConfigureHealthCheck(Settings)
            .ConfigureHttpRequestCorrelation(Settings)
            .ConfigureLogging(Settings)
            .ConfigureMediator(Settings)
            .ConfigureMinimalEndpoints(Settings)
            .ConfigureOpenApi(Settings)
            .ConfigureOpenTelemetry(Settings)
            .ConfigureOutputCache(Settings)
            .ConfigureRateLimiter(Settings)
            .ConfigureRequestTimeout(Settings)
            .ConfigureResilience(Settings)
            .ConfigureServiceDiscovery(Settings)
            .ConfigureValidation(Settings)
            .ConfigureWorkers(Settings)

            // Additional configuration should be the last one
            // since it can replace previous registered services.
            .ConfigureAdditionalServices(Settings);
    }

    private WebApplication CreateWebApplication() {
        // Here we are configuring the application middlewares
        // It's important that they respect a specific order
        // of registration to work properly.
        // Check ASP.NET Core documentation for more information.
        return Builder
            .Build()
            .UseLogging(Settings)
            .UseExceptionHandling(Settings)
            .UseHttpsRedirection(Settings)
            .UseHttpRequestCorrelation(Settings)

            // Important: UseRouting must be called here before
            // "UseEndpoints" or "UseAuthorization".
            .UseRouting(Settings)

            .UseCors(Settings)
            .UseRateLimiter(Settings)
            .UseOutputCache(Settings)
            .UseRequestTimeout(Settings)
            .UseAuth(Settings)
            .UseAntiforgery(Settings)
            .UseOpenApi(Settings)
            .UseScalar(Settings)
            .UseMinimalEndpoints(Settings)
            .UseHealthCheck(Settings)

            .UseBeforeStartup(Settings);
    }
}