using System.Reflection;
using Nameless.Web;
using Nameless.Web.Hosting;
using Nameless.Web.Http.Endpoints.Generator;
using Scalar.AspNetCore;

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

            settings.OpenApiRegistrationConfiguration = openapi => {
                openapi.RegisterOpenApiDocument("v1", _ => { });
                openapi.RegisterOpenApiDocument("v2", _ => { });
            };

            settings.ScalarRegistrationConfiguration = scalar => {
                scalar.ConfigureScalar = (configure, _) => {
                    configure.AddDocument("v1", "Microservice V1 Documentation", "/openapi/v1.json", isDefault: true);
                    configure.AddDocument("v2", "Microservice V2 Documentation", "/openapi/v2.json");
                };
            };

            settings.AdditionalServicesConfiguration = (services, _, _) => {
                services.RegisterAutoEndpoints();
            };

            settings.UseBeforeStartup = (_, route) => {
                route.MapAutoEndpoints();
            };

        }).Run();
    }
}
