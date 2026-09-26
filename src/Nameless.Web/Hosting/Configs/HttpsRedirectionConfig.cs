using Microsoft.AspNetCore.Builder;

namespace Nameless.Web.Hosting.Configs;

public static class HttpsRedirectionConfig {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the HTTPS redirection service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseHttpsRedirection(WebHostSettings settings) {
            if (settings.DisableHttpsRedirection) { return self; }

            self.UseHttpsRedirection();

            return self;
        }
    }
}
