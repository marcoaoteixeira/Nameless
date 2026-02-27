using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.RequestTimeouts;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures request timeout service with predefined expiration
        ///     policies for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds request timeout to the application's service
        ///     collection and registers some defaults policies for
        ///     one-second, five-seconds, fifteen-seconds, thirty-seconds and
        ///     one-minute expirations. For more information about request
        ///     timeout in ASP.NET Core, see <a href="https://learn.microsoft.com/en-us/aspnet/core/performance/timeouts">Request Timeout middleware in ASP.NET Core</a>
        /// </remarks>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterRequestTimeouts() {
            var options = self.Configuration.GetOptions<RequestTimeoutPolicyOptions>();

            return self.InnerRegisterRequestTimeouts(options);
        }

        /// <summary>
        ///     Configures request timeout service with predefined expiration
        ///     policies for the application.
        /// </summary>
        /// <remarks>
        ///     This method adds request timeout to the application's service
        ///     collection and registers some defaults policies for
        ///     one-second, five-seconds, fifteen-seconds, thirty-seconds and
        ///     one-minute expirations. For more information about request
        ///     timeout in ASP.NET Core, see <a href="https://learn.microsoft.com/en-us/aspnet/core/performance/timeouts">Request Timeout middleware in ASP.NET Core</a>
        /// </remarks>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterRequestTimeouts(Action<RequestTimeoutPolicyOptions> configure) {
            var options = ActionHelper.FromDelegate(configure);

            return self.InnerRegisterRequestTimeouts(options);
        }

        private WebApplicationBuilder InnerRegisterRequestTimeouts(RequestTimeoutPolicyOptions options) {
            self.Services.AddRequestTimeouts(opts => {
                foreach (var entry in options.Entries) {
                    opts.AddPolicy(entry.Name, entry.ToPolicy());
                }
            });

            return self;
        }
    }
}