using Microsoft.AspNetCore.Builder;

namespace Nameless.Microservices.StartUp;

public static partial class WebAppExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the HTTPS redirection service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseHttpsRedirection(WebAppSettings settings) {
            if (settings.DisableHttpsRedirection) { return self; }

            self.UseHttpsRedirection();

            return self;
        }
    }
}
