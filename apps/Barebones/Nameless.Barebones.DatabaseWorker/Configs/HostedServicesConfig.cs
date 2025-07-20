using Microsoft.EntityFrameworkCore;
using Nameless.Barebones.Domains;
using Nameless.Barebones.Worker.Workers;

namespace Nameless.Barebones.Worker.Configs;
public static class HostedServicesConfig {
    public static THostApplicationBuilder RegisterWorkerService<THostApplicationBuilder>(this THostApplicationBuilder self)
        where THostApplicationBuilder : IHostApplicationBuilder {

        self.Services.AddDbContext<ApplicationDbContext>(options => {
            // "barebones-db" is the PostgreSQL database configured in the Aspire App Host.
            options.UseNpgsql(self.Configuration.GetConnectionString("barebones-db"));
        });
        self.Services.AddHostedService<DatabaseMigrationWorker>();

        return self;
    }
}
