using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Configuration;
using Nameless.Web.Hosting.Options;
using Nameless.Web.OutputCache;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers output cache, configured through the configuration, and uses its middleware.
/// </summary>
public class OutputCacheWebHostFeature : HostFeature<OutputCacheFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var policies = builder.Configuration.GetPolicies<OutputCachePolicyOptions>();

        if (options.IncludeDefaultPolicies) { IncludeDefaultPolicies(policies); }

        builder.Services.AddOutputCache(outputCache => {
            foreach (var policy in policies.Where(item => !item.Value.Skip)) {
                outputCache.AddPolicy(
                    policy.Key,
                    policy.Value.SetDuration
                );
            }
        });
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseOutputCache();
    }

    private static void IncludeDefaultPolicies(Dictionary<string, OutputCachePolicyOptions> actual) {
        actual[Policies.OutputCache.OneSecond] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(1),
            Skip = false
        };

        actual[Policies.OutputCache.FiveSeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(5),
            Skip = false
        };

        actual[Policies.OutputCache.FifteenSeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(15),
            Skip = false
        };

        actual[Policies.OutputCache.ThirtySeconds] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromSeconds(30),
            Skip = false
        };

        actual[Policies.OutputCache.OneMinute] = new OutputCachePolicyOptions {
            Duration = TimeSpan.FromMinutes(1),
            Skip = false
        };
    }
}