using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;

namespace Nameless.Web.Cors;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures Cross-Origin Resource Sharing (CORS) policies for
        ///     the application.
        /// </summary>
        /// <returns>
        ///     This method register CORS policies using options specified in
        ///     the application's configuration. It adds both custom policies
        ///     and a permissive policy that allows any origin, header, and
        ///     method. Call this method during application configuration to
        ///     enable CORS support for HTTP requests.
        ///     See more at <a href="https://learn.microsoft.com/en-us/aspnet/core/security/cors">Enable Cross-Origin Request (CORS) in ASP.NET Core</a>
        /// </returns>
        public WebApplicationBuilder RegisterCors() {
            var options = self.Configuration.GetOptions<CorsPolicyOptions>();

            return self.InnerConfigureCors(options);
        }

        /// <summary>
        ///     Configures Cross-Origin Resource Sharing (CORS) policies for
        ///     the application.
        /// </summary>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     This method register CORS policies using options specified in
        ///     the application's configuration. It adds both custom policies
        ///     and a permissive policy that allows any origin, header, and
        ///     method. Call this method during application configuration to
        ///     enable CORS support for HTTP requests.
        ///     See more at <a href="https://learn.microsoft.com/en-us/aspnet/core/security/cors">Enable Cross-Origin Request (CORS) in ASP.NET Core</a>
        /// </returns>
        public WebApplicationBuilder RegisterCors(Action<CorsPolicyOptions> configure) {
            var options = ActionHelper.FromDelegate(configure);

            return self.InnerConfigureCors(options);
        }

        private WebApplicationBuilder InnerConfigureCors(CorsPolicyOptions options) {
            self.Services.AddCors(opts => {
                var entries = options.UseDefaultPolicies
                    ? CorsPolicyOptions.Defaults.Concat(options.Entries)
                    : options.Entries;

                foreach (var entry in entries) {
                    opts.AddPolicy(entry.Name, entry.ToPolicy());
                }
            });

            return self;
        }
    }
}