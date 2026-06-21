using Microsoft.AspNetCore.Builder;
using Nameless.Web.Scalar;

namespace Nameless.Web.Hosting.Configs;

public static class ScalarConfig {
    extension(WebApplication self) {
        /// <summary>
        ///     Use Scalar feature.
        /// </summary>
        /// <returns>
        ///     The current <see cref="WebApplication"/> instance so
        ///     other actions can be chained.
        /// </returns>
        public WebApplication UseScalar(WebHostSettings settings) {
            return settings.DisableScalar
                ? self
                : self.UseScalar(settings.ConfigureScalar);
        }
    }
}
