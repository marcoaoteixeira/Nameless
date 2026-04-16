using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Nameless.Web.HealthCheck;

public class HealthCheckOptions {
    public string? Name { get; set; }
    public HealthStatus FailureStatus { get; set; }
    public string[] Tags { get; set; } = [];
    public TimeSpan Timeout { get; set; }
}