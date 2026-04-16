using System.Reflection;
using Nameless.Web;
using Nameless.Web.Hosting;

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
        }).Run();
    }
}
