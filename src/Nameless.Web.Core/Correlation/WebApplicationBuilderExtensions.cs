using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Correlation;

/// <summary>
///     <see cref="WebApplicationBuilder"/> extension methods.
/// </summary>
public static class WebApplicationBuilderExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplicationBuilder"/>.
    /// </param>
    extension(WebApplicationBuilder self) {
        /// <summary>
        ///     Registers HTTP context correlation services.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterHttpContextCorrelation() {
            self.Services.Configure<HttpContextCorrelationOptions>(
                config: self.Configuration.GetSection<HttpContextCorrelationOptions>()
            );

            return self;
        }

        /// <summary>
        ///     Registers HTTP context correlation services.
        /// </summary>
        /// <param name="configure">
        ///     The configuration delegate.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplicationBuilder"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplicationBuilder RegisterHttpContextCorrelation(Action<HttpContextCorrelationOptions> configure) {
            self.Services.Configure(configure);

            return self;
        }
    }
}