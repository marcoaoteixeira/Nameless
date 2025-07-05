using System.Reflection;
using Nameless.Barebones.Api.Configs;
using Nameless.Barebones.Application;
using Nameless.Barebones.Infrastructure.Monitoring;
using Nameless.Web;
using Nameless.Web.Correlation;
using Nameless.Web.Discoverability;
using Nameless.Web.Endpoints;
using Nameless.Web.HealthChecks;
using Nameless.Web.Infrastructure;
using Nameless.Web.OpenApi;
using Nameless.Web.OpenTelemetry;
using Serilog;

namespace Nameless.Barebones.Api;

public static class EntryPoint {
    private static readonly Assembly[] Assemblies = [
        typeof(EntryPoint).Assembly, // Nameless.Microservices.Web
    ];

    public static void Main(string[] args) {
        WebApplication.CreateBuilder(args)
                      .RegisterServices()
                      .Build()
                      .ResolveServices()
                      .Run();
    }

    private static WebApplicationBuilder RegisterServices(this WebApplicationBuilder self) {
        // Doesn't matter the order how
        // the services are registered.

        return self.RegisterOpenTelemetry()
                   .RegisterHealthChecks()
                   .RegisterLogging()
                   .RegisterCorrelationAccessor()
                   .RegisterActivitySourceManager()
                   .RegisterAntiforgery()
                   .RegisterCors()
                   .RegisterDataProtection()
                   .RegisterRequestTimeouts()
                   .RegisterOutputCache(options => {
                       if (self.Environment.IsDevelopment()) {
                           options.AddPolicy(nameof(IgnoreCacheControlPolicy), policyBuilder => {
                               policyBuilder.AddPolicy<IgnoreCacheControlPolicy>();
                           });
                       }

                       options.AddPolicy(OutputCachePolicy.OpenApiDocumentation.PolicyName, policyBuilder => {
                           policyBuilder.Expire(OutputCachePolicy.OpenApiDocumentation.Expiration);
                       });
                   })
                   .RegisterAuthorization()
                   .RegisterJwtBearerAuthentication()
                   .RegisterRateLimiter()
                   .RegisterDiscoverability()
                   .RegisterMinimalEndpoints(options => {
                       options.Assemblies = Assemblies;
                       options.ConfigureOpenApi = openApiOpts => {
                           openApiOpts.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                           openApiOpts.AddOperationTransformer<DeprecateOpenApiOperationTransformer>();
                       };
                   });
    }

    private static WebApplication ResolveServices(this WebApplication self) {
        // From here on, all services should be resolved in correct order
        // to ensure proper initialization.

        self.UseExceptionHandler();

        // Enables HSTS (HTTP Strict Transport Security) in non-development environments
        if (!self.Environment.IsDevelopment()) {
            self.UseHsts();
        }

        self.UseSerilogRequestLogging();
        self.UseHttpsRedirection();
        self.UseCorrelation();
        self.UseOpenApi();
        self.UseRouting();
        self.UseCors();
        self.UseRateLimiter();
        self.UseOutputCache();
        self.UseAuthentication();
        self.UseAuthorization();
        self.UseRequestTimeouts();
        self.UseAntiforgery();
        self.UseMinimalEndpoints();
        self.UseHealthChecks(self.Environment);

        return self;
    }
}
