using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Hosting;
using Nameless.Web.Configuration;
using Nameless.Web.Hosting.Options;
using Nameless.Web.RequestTimeout;

namespace Nameless.Web.Hosting.Features;

/// <summary>
///     Registers request timeouts, configured through the configuration, and uses their middleware.
/// </summary>
public class RequestTimeoutWebHostFeature : HostFeature<RequestTimeoutFeatureOptions>, IWebHostFeature {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature);
        var policies = builder.Configuration.GetPolicies<RequestTimeoutPolicyOptions>();

        if (options.IncludeDefaultPolicies) { IncludeDefaultPolicies(policies); }

        builder.Services.AddRequestTimeouts(requestTimeouts => {
            foreach (var policy in policies.Where(item => !item.Value.Skip)) {
                requestTimeouts.AddPolicy(
                    policy.Key,
                    policy.Value.ToPolicy()
                );
            }
        });
    }

    /// <inheritdoc />
    public void Use(WebApplication app, HostFeatureContext context) {
        app.UseRequestTimeouts();
    }

    private static void IncludeDefaultPolicies(Dictionary<string, RequestTimeoutPolicyOptions> actual) {
        actual[Policies.RequestTimeout.OneSecond] = new RequestTimeoutPolicyOptions {
            ExpiresIn = TimeSpan.FromSeconds(1),
            HttpStatusCode = StatusCodes.Status408RequestTimeout,
            Skip = false
        };

        actual[Policies.RequestTimeout.FiveSeconds] = new RequestTimeoutPolicyOptions {
            ExpiresIn = TimeSpan.FromSeconds(5),
            HttpStatusCode = StatusCodes.Status408RequestTimeout,
            Skip = false
        };

        actual[Policies.RequestTimeout.FifteenSeconds] = new RequestTimeoutPolicyOptions {
            ExpiresIn = TimeSpan.FromSeconds(15),
            HttpStatusCode = StatusCodes.Status408RequestTimeout,
            Skip = false
        };

        actual[Policies.RequestTimeout.ThirtySeconds] = new RequestTimeoutPolicyOptions {
            ExpiresIn = TimeSpan.FromSeconds(30),
            HttpStatusCode = StatusCodes.Status408RequestTimeout,
            Skip = false
        };

        actual[Policies.RequestTimeout.OneMinute] = new RequestTimeoutPolicyOptions {
            ExpiresIn = TimeSpan.FromMinutes(1),
            HttpStatusCode = StatusCodes.Status408RequestTimeout,
            Skip = false
        };
    }
}
