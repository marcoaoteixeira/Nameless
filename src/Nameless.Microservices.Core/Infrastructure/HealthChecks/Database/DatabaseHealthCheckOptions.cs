namespace Nameless.Microservices.Infrastructure.HealthChecks.Database;

public class DatabaseHealthCheckOptions {
    public string Sql { get; set; } = string.Empty;
}