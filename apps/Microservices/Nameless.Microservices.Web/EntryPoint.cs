using System.Reflection;
using Nameless.Microservices.Infrastructure.OpenApi;
using Nameless.Microservices.Web.Configs;
using Nameless.Web.Discoverability;
using Nameless.Web.Endpoints;
using Nameless.Web.OpenApi;

namespace Nameless.Microservices.Web;

public static class EntryPoint {
    private static readonly Assembly[] Assemblies = [
        typeof(EntryPoint).Assembly, // Nameless.Microservices.Web
    ];

    public static void Main(string[] args) {
        WebApplication.CreateBuilder(args)
                      .ConfigureServices()
                      .Build()
                      .ResolveServices()
                      .Run();
    }

    private static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder self) {
        // Doesn't matter the order where the services are
        // registered.

        self.ConfigureMonitoring()
            .ConfigureServiceDiscovery()
            .ConfigureSecurity()
            .ConfigureMinimalEndpoint(options => {
                options.Assemblies = Assemblies;
                options.ConfigureOpenApi = openApiOpts => {
                    openApiOpts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                    openApiOpts.AddOperationTransformer<DeprecateOpenApiOperationTransformer>();
                };
            })
            .ConfigureOutputCache()
            .ConfigureAuth()
            .ConfigureRateLimiter();

        return self;
    }

    private static WebApplication ResolveServices(this WebApplication self) {
        // From here on, all services should be resolved in correct order
        // to ensure proper initialization.

        self.UseExceptionHandler();

        // Enables HSTS (HTTP Strict Transport Security) in non-development environments
        if (!self.Environment.IsDevelopment()) {
            self.UseHsts();
        }

        self.UseHttpsRedirection();

        self.UseOpenApi();

        self.UseRouting();

        self.UseCors();

        //self.UseRateLimiter();

        //self.UseOutputCache();

        //self.UseAuthentication();

        self.UseAuthorization();

        self.UseRequestTimeouts();

        //self.UseAntiforgery();

        self.UseMinimalEndpoints();

        //self.UseMonitoring();

        return self;
    }
}
