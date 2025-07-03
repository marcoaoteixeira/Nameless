using System.Reflection;
using Nameless.Identity.Web.Configs;
using Nameless.Web.Endpoints;
using Nameless.Web.OpenApi;

namespace Nameless.Identity.Web;

public static class EntryPoint {
    private static readonly Assembly[] Assemblies = [
        typeof(EntryPoint).Assembly, // Nameless.Identity.Web
    ];

    public static void Main(string[] args) {
        WebApplication.CreateBuilder(args)
                      .ConfigureMonitoringServices()
                      .RegisterMinimalEndpoints(options => {
                          options.Assemblies = Assemblies;
                          options.ConfigureOpenApi = openApiOpts => {
                              openApiOpts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                          };
                      })
                      .ConfigureOutputCacheServices()
                      .ConfigureAuthServices()
                      .ConfigureSecurityServices()

                      .Build() // Build the application

                      // From here, we can configure the middleware pipeline
                      .UseErrorHandlingServices()
                      .UseOpenApiServices()
                      .UseSecurityServices()
                      .UseMinimalEndpointServices()
                      .UseMonitoringServices()
                      .Run();
    }
}
