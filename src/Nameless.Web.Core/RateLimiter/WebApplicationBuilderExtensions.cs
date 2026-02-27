using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Nameless.Helpers;

namespace Nameless.Web.RateLimiter;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures rate limiting for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds the specified rate limiter policies to the
        ///     application's service collection. You can either register
        ///     specific policies through the <paramref name="registration"/>
        ///     delegate or use the auto discover functionality. For more info
        ///     on rate limiting in ASP.NET Core, see <a href="https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit">Rate limiting middleware in ASP.NET Core</a>
        /// </remarks>
        /// <param name="registration">
        ///     The registration settings delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterRateLimiter(Action<RateLimiterRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.Services.AddRateLimiter(opts => {
                foreach (var implementation in settings.RateLimiterPolicies) {
                    opts.AddPolicy(implementation.Key, implementation.Value);
                }
            });

            return self;
        }
    }

    extension(RateLimiterOptions self) {
        private void AddPolicy(string name, Type policyType) {
            var @interface = policyType.GetInterfacesThatCloses(typeof(IRateLimiterPolicy<>))
                                       .SingleOrDefault();

            var partitionKeyType = @interface?.GetGenericArguments()
                                             .SingleOrDefault();

            if (partitionKeyType is null) { return; }

            var handler = typeof(RateLimiterOptions)
                          .GetMethod(nameof(RateLimiterOptions.AddPolicy))?
                          .MakeGenericMethod(partitionKeyType);

            handler?.Invoke(
                obj: self,
                parameters: [name, Activator.CreateInstance(policyType)]
            );
        }
    }
}