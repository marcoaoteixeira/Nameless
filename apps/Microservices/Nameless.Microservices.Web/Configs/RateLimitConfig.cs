namespace Nameless.Microservices.Web.Configs;

public static class RateLimitConfig {
    public static WebApplicationBuilder ConfigureRateLimiter(this WebApplicationBuilder self) {
        self.Services.AddRateLimiter();

        return self;
    }
}
