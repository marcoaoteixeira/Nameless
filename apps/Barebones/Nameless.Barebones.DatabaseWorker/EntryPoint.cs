using Nameless.Barebones.Infrastructure.Monitoring;
using Nameless.Barebones.Worker.Configs;
using Nameless.Barebones.Worker.Workers;
using Nameless.Web.Discoverability;
using Nameless.Web.HealthChecks;
using Nameless.Web.OpenTelemetry;

namespace Nameless.Barebones.Worker;

public static class EntryPoint {
    public static void Main(string[] args) {
        Host.CreateApplicationBuilder(args)
            .RegisterOpenTelemetry(options => options.ActivitySources = [
                DatabaseMigrationWorker.ActivitySourceName
            ])
            .RegisterHealthChecks()
            .RegisterDiscoverability()
            .RegisterActivitySourceManager()
            .RegisterWorkerService()
            .Build()

            .Run();
    }
}