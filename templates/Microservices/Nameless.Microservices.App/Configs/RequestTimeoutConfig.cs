namespace Nameless.Microservices.App.Configs;

public static class RequestTimeoutConfig {
    public static WebApplicationBuilder ConfigureRequestTimeout(this WebApplicationBuilder self) {
        self.Services.AddRequestTimeouts(options => {
            options.AddPolicy(Constants.RequestTimeoutPolicies.ONE_SECOND, TimeSpan.FromSeconds(1));
            options.AddPolicy(Constants.RequestTimeoutPolicies.FIVE_SECONDS, TimeSpan.FromSeconds(5));
            options.AddPolicy(Constants.RequestTimeoutPolicies.FIFTEEN_SECONDS, TimeSpan.FromSeconds(15));
            options.AddPolicy(Constants.RequestTimeoutPolicies.THIRTY_SECONDS, TimeSpan.FromSeconds(30));
            options.AddPolicy(Constants.RequestTimeoutPolicies.ONE_MINUTE, TimeSpan.FromMinutes(1));
        });

        return self;
    }
}
