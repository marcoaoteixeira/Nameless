using Nameless.Web.HealthChecks;

namespace Nameless.Microservices.App.Configs;

public static class HealthCheckConfig {
    public static WebApplicationBuilder ConfigureHealthCheck(this WebApplicationBuilder self) {
        // Adds health checks to the application.
        // It will add a default liveness check to ensure the app is responsive.
        self.RegisterHealthChecks();

        return self;
    }

    public static WebApplication UseHealthCheck(this WebApplication self) {
        self.UseHealthChecks(self.Environment);

        return self;
    }
}
