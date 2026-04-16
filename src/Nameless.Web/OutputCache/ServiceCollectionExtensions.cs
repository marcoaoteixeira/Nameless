using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.OutputCache;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Configures output caching services with predefined cache
        ///     expiration policies for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds output caching to the application's service
        ///     collection and registers some defaults cache policies for
        ///     five-second and one-minute expiration durations. For more
        ///     information about output caching in ASP.NET Core,
        ///     see <a href="https://learn.microsoft.com/aspnet/core/performance/caching/output">Output caching middleware in ASP.NET Core</a>
        /// </remarks>
        /// <param name="configuration">
        ///     The configuration root object.
        /// </param>
        /// <param name="includeDefaultPolicies">
        ///     Whether it should include predefined policies.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterOutputCache(IConfiguration? configuration = null, bool includeDefaultPolicies = false) {
            self.AddOutputCache(builder => {
                var policies = configuration?.GetMultipleOptions<OutputCachePolicyOptions>() ?? [];

                if (includeDefaultPolicies) { IncludeDefaultPolicies(policies); }

                foreach (var policy in policies.Where(item => !item.Value.Skip)) {
                    builder.AddPolicy(
                        policy.Key,
                        policy.Value.ToPolicy
                    );
                }
            });

            return self;
        }
    }

    private static void IncludeDefaultPolicies(Dictionary<string, OutputCachePolicyOptions> actual) {
        actual[WebDefaults.OutputCachePolicies.OneSecond] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(1),
            Skip = false
        };

        actual[WebDefaults.OutputCachePolicies.FiveSeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(5),
            Skip = false
        };

        actual[WebDefaults.OutputCachePolicies.FifteenSeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(15),
            Skip = false
        };

        actual[WebDefaults.OutputCachePolicies.ThirtySeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(30),
            Skip = false
        };

        actual[WebDefaults.OutputCachePolicies.OneMinute] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromMinutes(1),
            Skip = false
        };
    }
}