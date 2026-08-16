using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nameless.Helpers;
using MS_HealthCheckRegistration = Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckRegistration;

namespace Nameless.Web.HealthCheck;

/// <summary>
///     Host application builder extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers health checks for the application.
        /// </summary>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterHealthCheck(Action<HealthCheckRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);
            var config = configuration?.GetOptions<HealthCheckConfiguration>() ?? new HealthCheckConfiguration();

            // Include the basic security for production environment
            // For more information, see: https://aspire.dev/fundamentals/health-checks/#non-development-environments
            self.AddRequestTimeouts(
                request => request.AddPolicy(
                    policyName: HealthCheckConfiguration.TimeoutPolicyName,
                    timeout: config.RequestTimeout
                )
            );

            self.AddOutputCache(
                outputCache => outputCache.AddPolicy(
                    name: HealthCheckConfiguration.TimeoutPolicyName,
                    build: policy => policy.Expire(config.OutputCacheTimeout)
                )
            );

            // Adding health checks endpoints to applications in non-development
            var builder = self.AddHealthChecks();

            // Add other health checks.
            foreach (var kvp in settings.HealthChecks) {
                self.TryAddSingleton(kvp.Key);

                builder.Add(new MS_HealthCheckRegistration(
                    name: kvp.Value.Name ?? kvp.Key.Name,
                    factory: provider => (IHealthCheck)provider.GetRequiredService(kvp.Key),
                    failureStatus: kvp.Value.FailureStatus,
                    tags: kvp.Value.Tags,
                    timeout: kvp.Value.Timeout
                ));
            }

            // Add a default liveness check to ensure app is responsive
            builder.AddCheck(
                name: "self",
                check: () => HealthCheckResult.Healthy(),
                tags: ["live"]
            );

            return self;
        }
    }
}