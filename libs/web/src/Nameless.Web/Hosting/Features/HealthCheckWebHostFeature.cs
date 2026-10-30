using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Nameless.Configuration;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.HealthCheck;
using Nameless.Web.HealthCheck.Reporting;
using Nameless.Web.Hosting.Options;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers the health checks and maps the liveness and readiness endpoints.
/// </summary>
public class HealthCheckWebHostFeature : HostFeature<HealthCheckFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);

        var healthCheckBuilder = builder.Services.AddHealthChecks();

        // Add other health checks.
        foreach (var healthCheck in options.HealthChecks) {
            builder.Services.TryAddScoped(healthCheck.Type);

            healthCheckBuilder.Add(new MS_HealthCheckRegistration(
                healthCheck.Name,
                provider => (IHealthCheck)provider.GetRequiredService(healthCheck.Type),
                healthCheck.FailureStatus,
                healthCheck.Tags,
                healthCheck.Timeout
            ));
        }

        // Add a default liveness check to ensure app is responsive
        healthCheckBuilder.AddCheck(
            name: "self",
            check: () => HealthCheckResult.Healthy(),
            // use tag to avoid call other configured health checks.
            tags: [HealthCheckConstants.Tags.Alive]
        );
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        var healthChecks = app.MapGroup(string.Empty);

        healthChecks
            .DisableRequestTimeout()
            .DisableRateLimiting()
            .AllowAnonymous();

        var config = app.Configuration
                        .GetSectionFor<HealthCheckOptions>()
                        .GetOrThrow<HealthCheckOptions>();

        // Liveness: no checks at all, just "process is alive" (cheap, safe)
        healthChecks.MapHealthChecks(
            pattern: config.LivenessPath,
            options: new MS_HealthCheckOptions {
                Predicate = registration => registration.Tags.Contains(HealthCheckConstants.Tags.Alive),
                ResponseWriter = JsonReportWriter.WriteAsync
            }
        );

        // Readiness: real dependency checks, tagged
        healthChecks.MapHealthChecks(
            pattern: config.ReadinessPath,
            options: new MS_HealthCheckOptions {
                Predicate = registration => registration.Tags.Overlaps(config.ReadinessTags),
                ResponseWriter = JsonReportWriter.WriteAsync
            }
        );
    }
}
