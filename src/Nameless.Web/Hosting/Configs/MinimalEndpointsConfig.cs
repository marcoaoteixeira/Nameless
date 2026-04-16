using Microsoft.AspNetCore.Builder;
using Nameless.Web.Endpoints;

namespace Nameless.Web.Hosting.Configs;

public static class MinimalEndpointsConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures the application to use the default minimal endpoint
        ///     infrastructure, enabling endpoint discovery and related
        ///     features.
        /// </summary>
        /// <remarks>
        ///     This method enables support for minimal endpoints, including
        ///     automatic discovery of endpoint definitions within the
        ///     specified assemblies. It also configures related features such
        ///     API Explorer and API versioning.
        /// </remarks>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureMinimalEndpoints(WebHostSettings settings) {
            if (settings.DisableMinimalEndpoints) { return self; }

            self.RegisterEndpoints(
                WebHostSettingsHelper.Join(
                    settings.MinimalEndpointsRegistrationConfiguration,
                    settings.Assemblies
                )
            );

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the minimal endpoints service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseMinimalEndpoints(WebHostSettings settings) {
            if (settings.DisableMinimalEndpoints) { return self; }

            self.UseEndpoints();

            return self;
        }
    }
}
