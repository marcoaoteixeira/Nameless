using System.Reflection;
using Asp.Versioning;
using Nameless.Web;
using Nameless.Web.Hosting;
using Nameless.Web.Http.Endpoints.Generated;

namespace Nameless.Microservice.Api;

public class EntryPoint {
    private static readonly Assembly[] SupportAssemblies = [
        typeof(AssemblyMarkerCore).Assembly,
        typeof(AssemblyMarkerImpl).Assembly,
        typeof(AssemblyMarkerWeb).Assembly,
    ];

    public static void Main(params string[] args) {
        WebHostFactory.Create(settings => {
            settings.Args = args;
            settings.Assemblies = SupportAssemblies;

            settings.AdditionalServicesConfiguration = (services, _, _) => {
                services.RegisterAutoEndpoints();
            };

            settings.UseBeforeStartup = (_, route) => {
                route.MapGroup("")
                     .WithDescription("")
                     .WithDisplayName("")
                     .WithSummary("")
                     
                     ;


                route.MapGet("/something", _ => throw new InvalidOperationException())
                     .AllowCookieRedirect()
                     .DisableCookieRedirect()

                     

                     .DisableAntiforgery()
                     
                     .AllowAnonymous()
                     .RequireAuthorization("")

                     .CacheOutput("")
                     
                     .DisableHttpMetrics()
                     
                     .CacheOutput(policy => policy.NoCache())
                     // counterpart disable output cache created

                     .RequireRateLimiting("")
                     .DisableRateLimiting()

                     .DisableRequestTimeout()
                     .WithRequestTimeout("")

                     

                     .WithName("asas")
                     
                     .DisableValidation()

                     .RequireCors("")
                     // To disable CORS use DisableCorsAttribute
                     
                     .ProducesProblem(100)
                     .ProducesValidationProblem()

                     .WithDescription("")
                     .WithDisplayName("")
                     .WithName("")
                     .WithSummary("")

                     .MapToApiVersion(new ApiVersion(1,2))
                     .HasDeprecatedApiVersion(1)

                     .WithGroupName("");

                route.MapAutoEndpoints();
            };

        }).Run();
    }
}
