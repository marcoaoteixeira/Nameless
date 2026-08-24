using Nameless.Web.Hosting;

namespace Nameless.Microservices.Worker;

public class EntryPoint {
    public static void Main(string[] args) {
        WebHostFactory.Create(settings => {
            settings.Args = args;
            settings.Assemblies = [
                typeof(EntryPoint).Assembly,
                typeof(AssemblyMarkerCommon).Assembly
            ];

            DisableServices(settings);
        }).Run();
    }

    private static void DisableServices(WebHostSettings settings) {
        settings.DisableBootstrap = false;
        settings.DisableExceptionHandling = false;
        settings.DisableLogging = false;
        settings.DisableMediator = false;
        settings.DisableOpenTelemetry = false;
        settings.DisableResilience = false;
        settings.DisableValidator = false;
        settings.DisablePeriodicWorkers = false;
    }
}
