using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Scalar.AspNetCore;

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
            if (settings.DisableScalar) { return self; }

            // Do not expose Scalar on PROD environment
            if (self.Environment.IsProduction()) { return self; }

            var scalar = ActionHelper.FromDelegate(settings.ScalarRegistrationConfiguration);
            var combination = Delegate.Combine(DefaultScalarConfiguration, scalar.ConfigureScalar);

            self.MapScalarApiReference((Action<ScalarOptions>)combination);

            return self;

            void DefaultScalarConfiguration(ScalarOptions options, HttpContext _) {
                options
                    .WithTitle(self.Environment.ApplicationName)
                    .WithTheme(ScalarTheme.BluePlanet)
                    .WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl);
            }
        }
    }
}
