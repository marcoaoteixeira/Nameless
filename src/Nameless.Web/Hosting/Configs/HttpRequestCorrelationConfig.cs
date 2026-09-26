using Microsoft.AspNetCore.Builder;
using Nameless.Web.Correlation;

namespace Nameless.Web.Hosting.Configs;

public static class HttpRequestCorrelationConfig {
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Configures HttpContextCorrelation feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder ConfigureHttpRequestCorrelation(WebHostSettings settings) {
            if (settings.DisableHttpRequestCorrelation) { return self; }

            self.Services.RegisterHttpRequestCorrelation(
                self.Configuration
            );

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
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseHttpRequestCorrelation(WebHostSettings settings) {
            if (settings.DisableHttpRequestCorrelation) { return self; }

            self.UseHttpRequestCorrelation();

            return self;
        }
    }
}
