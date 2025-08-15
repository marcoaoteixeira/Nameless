using System.Reflection;
using Nameless.Microservices.App.Configs;
using Nameless.Web.Correlation;
using Nameless.Web.Endpoints;

namespace Nameless.Microservices.App;

/// <summary>
///     The application entry point.
/// </summary>
public class EntryPoint {
    // Many services use auto-discovery to locate their implementations
    // and supporting classes. This process requires the assemblies
    // available to the application. Specify the relevant assemblies
    // here to ensure they are loaded and available for discovery.
    private static readonly Assembly[] SupportAssemblies = [
        typeof(EntryPoint).Assembly,
        typeof(IEndpoint).Assembly,
    ];

    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    /// <param name="args">
    ///     The command line arguments passed to the application.
    /// </param>
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // When registering services, we do not need to put
        // them in any specific order, but at least we should
        // keep them grouped by their purpose.
        builder
            .ConfigureCommon()
            .ConfigureDbContext()
            .ConfigureValidation(SupportAssemblies)
            .ConfigureMediator(SupportAssemblies)
            .ConfigureMinimalEndpoints(SupportAssemblies)
            .ConfigureAuth()
            .ConfigureDataProtection()
            .ConfigureLogging()
            .ConfigureRateLimit()
            .ConfigureRequestTimeout()
            .ConfigureOutputCache()
            .ConfigureCors()
            .ConfigureHealthCheck();

        var app = builder.Build();

        // When setting up the application middlewares,
        // we should be aware that the order of these middlewares
        // is important.

        app.UseDbContext();
        app.UseExceptionHandler();
        app.UseLogging();
        app.UseHttpsRedirection();
        app.UseHttpContextCorrelation();
        app.UseOpenApi();

        // Important: UseRouting must be called here before
        // UseEndpoints or UseAuthorization.
        app.UseRouting();

        app.UseCors();
        app.UseRateLimiter();
        app.UseOutputCache();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRequestTimeouts();
        app.UseAntiforgery();
        app.UseMinimalEndpoints();
        app.UseHealthCheck();

        app.Run();
    }
}
