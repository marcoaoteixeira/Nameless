using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Configuration;
using Nameless.Web.Cors;
using Nameless.Web.Hosting.Options;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers CORS, configured through the configuration, and uses its middleware.
/// </summary>
public class CorsWebHostFeature : HostFeature<CorsFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var policies = builder.Configuration.GetPolicies<CorsPolicyOptions>();

        if (options.IncludeBuiltinPolicies) { IncludeBuiltinPolicies(policies); }

        builder.Services.AddCors(cors => {
            foreach (var policy in policies.Where(item => !item.Value.Skip)) {
                cors.AddPolicy(policy.Key, policy.Value.ToPolicy());
            }
        });
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseCors();
    }

    private static void IncludeBuiltinPolicies(Dictionary<string, CorsPolicyOptions> actual) {
        actual[Policies.Cors.AllowEverything] = new CorsPolicyOptions {
            Headers = "*",
            Methods = "*",
            Origins = "*",
            SupportsCredentials = false,
            PreflightMaxAge = null,
            Skip = false
        };
    }
}
