using Microsoft.AspNetCore.Builder;

namespace Nameless.Web.Hosting.Configs;

/// <summary>
///     Configure routing.
/// </summary>
public static class RoutingConfig {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the routing service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebHostSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRouting(WebHostSettings settings) {
            if (settings.DisableRouting) { return self; }

            settings.UseBeforeRouting(self);

            self.UseRouting();

            return self;
        }
    }
}
