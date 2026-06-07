using System.Reflection;
using Nameless.Web;
using Nameless.Web.Hosting;
using Nameless.Web.Http.Endpoints;

namespace Nameless.Microservices.Api;

public static class EntryPoint {
    private static Assembly[] SupportAssemblies => [
        typeof(EntryPoint).Assembly,
        typeof(AssemblyMarkerCore).Assembly,
        typeof(AssemblyMarkerImpl).Assembly,
        typeof(AssemblyMarkerWeb).Assembly
    ];

    public static void Main(params string[] args) {
        var host = WebHostFactory.Create(settings => {
            settings.Args = args;
            settings.Assemblies = SupportAssemblies;

            settings.ConfigureAdditionalServices = ConfigureAdditionalServices;
            settings.ExecuteBeforeStartup = ExecuteBeforeStartup;

            DisableServices(settings);
        });

        host.Run();
    }

    private static void ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment) {
        services.RegisterAutoEndpoints();
    }

    private static void ExecuteBeforeStartup(IApplicationBuilder app) {
        const string EndpointRouteBuilderKey = "__EndpointRouteBuilder";

        if (app.Properties.TryGetValue(EndpointRouteBuilderKey, out var output) && output is IEndpointRouteBuilder builder) {
            builder.MapAutoEndpoints();
        }
    }

    private static void DisableServices(WebHostSettings settings) {
        settings.DisableAntiforgery = true;
        settings.DisableDataProtection = true;
        settings.DisableOutputCache = true;
    }
}