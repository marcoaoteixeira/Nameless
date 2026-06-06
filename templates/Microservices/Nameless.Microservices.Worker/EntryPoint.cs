using Nameless.Web.Hosting;
using Org.BouncyCastle.Math.EC.Endo;

namespace Nameless.Microservices.Worker;

public static class EntryPoint {
    public static void Main(params string[] args) {
        var host = WebHostFactory.Create(settings => {
            settings.Args = args;

            // define support assemblies

            DisableServices(settings);
        });

        host.Run();
    }

    private static void DisableServices(WebHostSettings settings) {
        settings.DisableAntiforgery = true;
        settings.DisableDataProtection = true;
        settings.DisableOutputCache = true;
        settings.DisableRequestTimeouts = true;
    }
}
