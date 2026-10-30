using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.RateLimiter;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers the request rate limiter, scanning the context assemblies, and uses its middleware.
/// </summary>
public class RateLimiterWebHostFeature : HostFeature<RateLimiterFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);
        var policies = new Dictionary<string, Type>(options.RateLimiterPolicies);

        if (options.IncludeDefaultPolicies) { IncludeDefaultPolicies(policies); }

        builder.Services.AddRateLimiter(rateLimiter => {
            foreach (var kvp in policies) {
                rateLimiter.RegisterPolicy(kvp.Key, kvp.Value);
            }
        });
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseRateLimiter();
    }

    private static void IncludeDefaultPolicies(Dictionary<string, Type> actual) {
        actual[Policies.RateLimiter.SlidingWindow] = typeof(DefaultSlidingWindowRateLimiterPolicy);
    }
}
