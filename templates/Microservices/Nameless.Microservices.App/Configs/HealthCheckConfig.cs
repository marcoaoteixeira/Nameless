using Nameless.Microservices.App.Infrastructure.Sqlite;
using Nameless.Web.HealthChecks;

namespace Nameless.Microservices.App.Configs;

public static class HealthCheckConfig {
    public static WebApplicationBuilder ConfigureHealthCheck(this WebApplicationBuilder self) {
        // Adds a health check named "self" that always reports a healthy status,
        // tagged with "live" for liveness probes.
        // This is just to check if the microservice still running.
        self.RegisterHealthChecks(configure => {
            configure.RegisterHealthCheck<SqliteHealthCheck>();
        });

        return self;
    }

    public static WebApplication UseHealthCheck(this WebApplication self) {
        self.UseHealthChecks(self.Environment);

        return self;
    }
}
