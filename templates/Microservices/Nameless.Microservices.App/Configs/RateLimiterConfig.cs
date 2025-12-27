using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Nameless.Microservices.App.Configs;

public static class RateLimiterConfig {
    extension(WebApplicationBuilder self) {
        // See more at https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-10.0
        public WebApplicationBuilder ConfigureRateLimit() {
            self.Services.AddRateLimiter(options => {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddSlidingWindowLimiter(Constants.RateLimitPolicies.SLIDING_WINDOW, rateLimiterOptions => {
                    rateLimiterOptions.Window = TimeSpan.FromSeconds(15);
                    rateLimiterOptions.PermitLimit = 5; // Maximum number of requests allowed in the window
                    rateLimiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    rateLimiterOptions.SegmentsPerWindow = 1; // Number of segments in the window
                });
            });

            return self;
        }
    }
}
