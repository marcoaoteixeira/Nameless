using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.OutputCache;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/> instance.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures output caching services with predefined cache
        ///     expiration policies for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds output caching to the application's service
        ///     collection and registers some defaults cache policies for
        ///     five-second and one-minute expiration durations. For more
        ///     information about output caching in ASP.NET Core,
        ///     see <a href="https://learn.microsoft.com/aspnet/core/performance/caching/output">Output caching middleware in ASP.NET Core</a>
        /// </remarks>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterOutputCache(Action<OutputCachePolicyOptions> configure) {
            return self.InnerRegisterOutputCache(
                ActionHelper.FromDelegate(configure)
            );
        }

        /// <summary>
        ///     Configures output caching services with predefined cache
        ///     expiration policies for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds output caching to the application's service
        ///     collection and registers some defaults cache policies for
        ///     five-second and one-minute expiration durations. For more
        ///     information about output caching in ASP.NET Core,
        ///     see <a href="https://learn.microsoft.com/aspnet/core/performance/caching/output">Output caching middleware in ASP.NET Core</a>
        /// </remarks>
        /// <param name="configuration">
        ///     The configuration root object.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterOutputCache(IConfiguration configuration) {
            return self.InnerRegisterOutputCache(
                configuration.GetOptions<OutputCachePolicyOptions>()
            );
        }

        private WebApplicationBuilder InnerRegisterOutputCache(OutputCachePolicyOptions options) {
            self.Services.AddOutputCache(outputCache => {
                foreach (var policy in options.Entries) {
                    outputCache.AddPolicy(policy.Name, builder
                        => builder.Expire(policy.Duration)
                    );
                }
            });

            return self;
        }
    }
}