using Nameless.Web.Infrastructure;

namespace Nameless.Identity.Web.Configs;

/// <summary>
///     Configures output caching for the application.
/// </summary>
/// <remarks>
///     A call to <c>UseOutputCache</c> must be placed after <c>UseRouting</c>
///     and before <c>UseAuthorization</c> in the middleware pipeline.
///     See <see cref="EndpointsConfig.UseMinimalEndpointServices"/> for more
///     details.
/// </remarks>
public static class OutputCacheConfig {
    public static WebApplicationBuilder ConfigureOutputCacheServices(this WebApplicationBuilder self) {
        self.Services
            .AddOutputCache(options => {
                if (self.Environment.IsDevelopment()) {
                    options.AddPolicy(nameof(IgnoreCacheControlPolicy), policyBuilder => {
                        policyBuilder.AddPolicy<IgnoreCacheControlPolicy>();
                    });
                }

                options.AddPolicy(OutputCachePolicy.OpenApi.PolicyName, policyBuilder => {
                    policyBuilder.Expire(OutputCachePolicy.OpenApi.Expiration);
                });
                options.AddPolicy(OutputCachePolicy.OneMinute.PolicyName, policyBuilder => {
                    policyBuilder.Expire(OutputCachePolicy.OneMinute.Expiration);
                });
            });
        return self;
    }
}
