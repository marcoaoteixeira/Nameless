using System.Reflection;
using Nameless.Web;
using Nameless.Web.Hosting;

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

            DisableServices(settings);
        });

        host.Run();
    }

    private static void DisableServices(WebHostSettings settings) {
        settings.DisableAntiforgery = true;
        settings.DisableDataProtection = true;
        settings.DisableOutputCache = true;
    }
}
