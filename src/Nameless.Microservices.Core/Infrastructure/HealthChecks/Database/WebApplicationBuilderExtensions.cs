using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

public static class WebApplicationBuilderExtensions {
    extension(WebApplicationBuilder self) {
        public WebApplicationBuilder RegisterDatabaseHealthCheck() {
            self.Services.Configure<DatabaseHealthCheckOptions>(
                self.Configuration
            );

            self.Services
                .AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>(nameof(DatabaseHealthCheck));

            self.Services
                .AddTransient<IDbConnectionFactory, DbConnectionFactory>();

            return self;
        }
    }
}
