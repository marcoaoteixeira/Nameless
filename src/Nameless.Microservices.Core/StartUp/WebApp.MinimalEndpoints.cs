using Microsoft.AspNetCore.Builder;
using Nameless.Web.MinimalEndpoints;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
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
        public WebApplicationBuilder MinimalEndpointsConfig(WebAppSettings settings) {
            if (settings.DisableMinimalEndpoints) { return self; }

            self.RegisterMinimalEndpoints(settings.ConfigureMinimalEndpoints ?? DefaultConfiguration);

            return self;

            void DefaultConfiguration(MinimalEndpointsRegistrationSettings opts) {
                opts.IncludeAssemblies(settings.Assemblies);
            }
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
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseMinimalEndpoints(WebAppSettings settings) {
            if (settings.DisableMinimalEndpoints) { return self; }

            self.UseMinimalEndpoints();

            return self;
        }
    }
}
