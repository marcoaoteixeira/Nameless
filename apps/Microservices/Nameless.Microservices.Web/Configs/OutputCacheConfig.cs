using Nameless.Microservices.Application;
using Nameless.Web.Infrastructure;

namespace Nameless.Microservices.Web.Configs;

/// <summary>
///     Configures output caching for the application.
/// </summary>
public static class OutputCacheConfig {
    public static WebApplicationBuilder ConfigureOutputCache(this WebApplicationBuilder self) {
        self.Services
            .AddOutputCache(options => {
                if (self.Environment.IsDevelopment()) {
                    options.AddPolicy(nameof(IgnoreCacheControlPolicy), policyBuilder => {
                        policyBuilder.AddPolicy<IgnoreCacheControlPolicy>();
                    });
                }

                options.AddPolicy(OutputCachePolicy.OpenApiDocumentation.PolicyName, policyBuilder => {
                    policyBuilder.Expire(OutputCachePolicy.OpenApiDocumentation.Expiration);
                });
            });
        return self;
    }
}
