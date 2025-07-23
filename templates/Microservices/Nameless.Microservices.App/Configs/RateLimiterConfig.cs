using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Nameless.Microservices.App.Configs;

public static class RateLimiterConfig {
    public static WebApplicationBuilder ConfigureRateLimit(this WebApplicationBuilder self) {
        self.Services.AddRateLimiter(options => {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddSlidingWindowLimiter(Constants.RateLimitPolicies.SLIDING_WINDOW, rateLimiterOptions => {
                rateLimiterOptions.Window = TimeSpan.FromSeconds(60);
                rateLimiterOptions.PermitLimit = 100; // Maximum number of requests allowed in the window
                rateLimiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                rateLimiterOptions.SegmentsPerWindow = 5; // Number of segments in the window
            });
        });

        return self;
    }
}
