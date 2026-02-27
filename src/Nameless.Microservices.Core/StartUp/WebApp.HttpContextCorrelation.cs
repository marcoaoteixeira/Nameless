using Microsoft.AspNetCore.Builder;
using Nameless.Web.Correlation;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures HttpContextCorrelation feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder HttpContextCorrelationConfig(WebAppSettings settings) {
            if (settings.DisableHttpContextCorrelation) { return self; }

            self.RegisterHttpContextCorrelation();

            return self;
        }
    }

    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the request correlation ID service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseHttpContextCorrelation(WebAppSettings settings) {
            if (settings.DisableHttpContextCorrelation) { return self; }

            self.UseHttpContextCorrelation();

            return self;
        }
    }
}
