namespace Nameless.Microservices.App.Configs;

public static class OutputCacheConfig {
    public static WebApplicationBuilder ConfigureOutputCache(this WebApplicationBuilder self) {
        self.Services.AddOutputCache(options => {
            options.AddPolicy(Constants.OutputCachePolicies.ONE_MINUTE, builder => {
                builder.Expire(TimeSpan.FromMinutes(1));
            });
        });

        return self;
    }
}
