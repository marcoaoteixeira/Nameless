using Microsoft.AspNetCore.Builder;

namespace Nameless.Microservices.StartUp;

/// <summary>
///     Configure routing.
/// </summary>
public static partial class WebAppExtensions {
    /// <param name="self">
    ///     The current <see cref="WebApplication"/> instance.
    /// </param>
    extension(WebApplication self) {
        /// <summary>
        ///     Uses the routing service.
        /// </summary>
        /// <param name="settings">
        ///     The <see cref="WebAppSettings"/> instance.
        /// </param>
        /// <returns>
        ///     The current <see cref="WebApplication"/> so other actions
        ///     can be chained.
        /// </returns>
        public WebApplication UseRouting(WebAppSettings settings) {
            if (settings.DisableRouting) { return self; }

            settings.UseBeforeRouting(self);

            self.UseRouting();

            return self;
        }
    }
}
