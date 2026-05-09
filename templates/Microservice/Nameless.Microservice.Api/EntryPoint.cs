using System.Reflection;
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
                route.MapAutoEndpoints();
            };
        }).Run();
    }
}
