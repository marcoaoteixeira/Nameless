namespace Nameless.Microservices.App.Configs;

public static class OutputCacheConfig {
    extension(WebApplicationBuilder self) {
        // See more at: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-10.0
        public WebApplicationBuilder ConfigureOutputCache() {
            self.Services.AddOutputCache(options => {
                options.AddPolicy(Constants.OutputCachePolicies.ONE_MINUTE, builder => {
                    builder.Expire(TimeSpan.FromMinutes(1));
                });
            });

            return self;
        }
    }
}
