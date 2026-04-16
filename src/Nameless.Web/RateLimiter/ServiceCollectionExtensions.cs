using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.RateLimiter;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
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
        ///     The current <see cref="IServiceCollection"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterRateLimiter(Action<RateLimiterRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.AddRateLimiter(opts => {
                var policies = GetRateLimiterPolicies(settings);

                foreach (var policy in policies) {
                    opts.AddPolicy(
                        policy.Key,
                        policy.Value
                    );
                }
            });

            return self;
        }
    }

    private static IReadOnlyDictionary<string, Type> GetRateLimiterPolicies(RateLimiterRegistration settings) {
        var service = typeof(IRateLimiterPolicy<>);

        return settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(service, includeGenericDefinition: false)
                      .ToDictionary(
                          keySelector: RateLimiterPolicyAttribute.GetName,
                          elementSelector: type => type
                      )
            : settings.RateLimiterPolicies;
    }

    extension(RateLimiterOptions self) {
        private void AddPolicy(string name, Type policyType) {
            var @interface = policyType.GetInterfacesThatCloses(typeof(IRateLimiterPolicy<>))
                                       .SingleOrDefault();

            var partitionKeyType = @interface?.GetGenericArguments()
                                             .SingleOrDefault();

            if (partitionKeyType is null) { return; }

            var handler = RateLimiterOptionsHandlerCache.Handler
                                                        .MakeGenericMethod(partitionKeyType);

            handler.Invoke(
                obj: self,
                parameters: [
                    name,
                    Activator.CreateInstance(policyType)
                ]
            );
        }
    }
}